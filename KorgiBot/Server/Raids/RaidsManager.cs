using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using KorgiBot.Configs;
using KorgiBot.Server.Raids.Commands;
using Timer = System.Timers.Timer;

namespace KorgiBot.Server.Raids
{
	public class RaidsManager
	{
		private readonly Bot _bot;
		private readonly RaidsConfig _raidsConfig;
		private readonly RaidsBackupConfig _backup;
		private readonly RaidsCommandsManager _commandsManager;

		private List<Timer> _timersToDeleteRaid = new();

		public ConcurrentDictionary<ulong, RaidProvider> ActiveRaids { get; private set; } = new();

		public RaidsManager(Bot bot, RaidsConfig raidsConfig, RaidsBackupConfig backup)
		{
			_bot = bot;
			_raidsConfig = raidsConfig;
			_backup = backup;
			_commandsManager = new RaidsCommandsManager(_bot, this, _raidsConfig);

			_bot.MessageCreated += async (source, args) => await HandleMessageCreated(args);

			Task.Run(() => _commandsManager.Initialize());
		}

		public async Task CreateRaid(ulong creatorId, DiscordChannel channel, string description, string startTime, string rawMembers, int firstRequired)
		{
			var roles = new List<RaidRole>();
			var number = 1;
			foreach (Match line in Regex.Matches(rawMembers, RaidPatterns.Role))
			{
				var split = line.Value.Split('-');
				var roleName = string.Join("", split[0].Skip(split[0].IndexOf('.') + 1));
				ulong roleOwnerId = split[1] == string.Empty ? 0 : ulong.Parse(Regex.Match(split[1], RaidPatterns.RoleId).Value);

				roles.Add(new RaidRole(number++, roleName, roleOwnerId));
			}

			var raid = new Raid(description, startTime, creatorId, roles, firstRequired, _raidsConfig.AdminRoles);
			var messages = raid.GetPreparedMessagesToSend();
			var sentMessages = new List<DiscordMessage>() { await _bot.SendMessageAsync(channel, messages.First()) };
			var thread = await sentMessages.First().CreateThreadAsync(startTime, AutoArchiveDuration.Day);

			foreach (var message in messages.Skip(1))
			{
				sentMessages.Add(await _bot.SendMessageAsync(thread, message));
			}

			var raidProvider = new RaidProvider(new RaidInfo(channel.Id, thread.Id, sentMessages.Select(msg => msg.Id).ToList()), raid);
			ActiveRaids.TryAdd(thread.Id, raidProvider);

			await OnMembersUpdate(raidProvider);

			RunDeleteTimer(thread.Id);
		}

		public async Task<bool> TryAppendChanges(ulong threadId, string changes)
		{
			if (!ActiveRaids.TryGetValue(threadId, out var raidProvider)) return false;

			var deletes = Regex.Matches(changes, RaidPatterns.RoleDelete).Select(match => match.Value);
			var additions = Regex.Matches(changes, RaidPatterns.Role).Select(match => match.Value);

			var result = false;
			foreach (var delete in deletes)
			{
				result = TryRemoveRole(threadId, int.Parse(delete.Substring(1)));
				if (!result) break;
			}

			foreach (var addition in additions)
			{
				var split = addition.Split('-');
				var number = int.Parse(string.Join("", split[0].TakeWhile(c => c != '.')));
				var roleName = string.Join("", split[0].Skip(split[0].IndexOf('.') + 1));
				ulong roleOwnerId = split[1] == string.Empty ? 0 : ulong.Parse(Regex.Match(split[1], RaidPatterns.RoleId).Value);

				result = TryAddRole(threadId, roleName, roleOwnerId, number);
				if (!result) break;
			}

			raidProvider.Raid.UpdateNumbers();
			await OnMembersUpdate(raidProvider);

			return result;
		}

		public async Task<bool> TryRemoveRaid(ulong threadId)
		{
			if (!ActiveRaids.TryGetValue(threadId, out var raidProvider)) return false;

			ActiveRaids.Remove(threadId, out _);

			await _bot.DeleteMessageAsync(raidProvider.Info.ChannelId, raidProvider.Info.MessageIds.First());

			try
			{
				await _bot.DeleteThreadAsync(raidProvider.Info.ThreadId);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while deletion raid thread\n{e}");
			}			

			return true;
		}

		public bool RaidExists(ulong threadId)
		{
			return ActiveRaids.TryGetValue(threadId, out _);
		}

		public bool TryAddRole(ulong threadId, string roleName, ulong userId, int number = 0)
		{
			if (!ActiveRaids.TryGetValue(threadId, out var raidProvider)) return false;

			raidProvider.Raid.AddOrUpdateRole(roleName, userId, number);
			return true;
		}

		public bool TryRemoveRole(ulong threadId, int number)
		{
			if (!ActiveRaids.TryGetValue(threadId, out var raidProvider)) return false;

			return raidProvider.Raid.TryRemoveRole(number, false);
		}

		public async Task<bool> TryAddMember(DiscordChannel thread, DiscordMember source, int number, DiscordMember target = null)
		{
			if (!ActiveRaids.TryGetValue(thread.Id, out var raidProvider)) return false;

			var result = raidProvider.Raid.TryAddMember(source, number, target);
			if (result) await OnMembersUpdate(raidProvider);
			
			return result;
		}

		public async Task<bool> TryRemoveMember(DiscordChannel thread, DiscordMember source, int number = 0)
		{
			if (!ActiveRaids.TryGetValue(thread.Id, out var raidProvider)) return false;

			var result = raidProvider.Raid.TryRemoveMember(source, number);
			if (result) await OnMembersUpdate(raidProvider);

			return result;
		}

		public async Task Recover()
		{
			ActiveRaids = new();
			var notActualRaids = new List<ulong>();

			foreach (var pair in _backup.Raids)
			{
				var channel = await _bot.GetChannelAsync(pair.Value.Info.ChannelId);
				var thread = await _bot.GetChannelAsync(pair.Value.Info.ThreadId);
				if (channel == null || thread == null)
				{
					notActualRaids.Add(pair.Key);
					continue;
				}

				var messagesToBackup = pair.Value.Info.MessageIds;
				var messages = new List<DiscordMessage>() { await _bot.GetMessageAsync(channel, messagesToBackup.First()) };
				if (messages.Any(msg => msg == null))
				{
					notActualRaids.Add(pair.Key);
					continue;
				}

				messages.AddRange(messagesToBackup.Skip(1).Select(async messageId => await _bot.GetMessageAsync(thread, messageId)).Select(t => t.Result).ToList());

				ActiveRaids.TryAdd(pair.Key, new RaidProvider(new RaidInfo(channel.Id, thread.Id, messages.Select(msg => msg.Id).ToList()), pair.Value.Raid));

				RunDeleteTimer(thread.Id);
			}

			foreach (var toRemove in notActualRaids)
			{
				_backup.Raids.Remove(toRemove, out _);
			}

			_backup.Save();
		}

		public async Task UpdateMembers(ulong threadId)
		{
			if (!ActiveRaids.TryGetValue(threadId, out var raidProvider)) return;

			await OnMembersUpdate(raidProvider);
		}

		private async Task HandleMessageCreated(MessageCreateEventArgs args)
		{
			if (!ActiveRaids.TryGetValue(args.Channel.Id, out _)) return;

			await _commandsManager.HandleCommand(args);
		}

		private async Task OnMembersUpdate(RaidProvider raidProvider)
		{
			UpdateBackup();

			var messagesToUpdate = raidProvider.Raid.GetPreparedMessagesToSend();
			messagesToUpdate[0] = $"Id: {raidProvider.Info.ThreadId}\n\n{messagesToUpdate[0]}";

			for (int i = 0; i < raidProvider.Info.MessageIds.Count; i++)
			{
				var channelId = i == 0 ? raidProvider.Info.ChannelId : raidProvider.Info.ThreadId;
				await _bot.EditMessageAsync(channelId, raidProvider.Info.MessageIds[i], messagesToUpdate[i]);
			}
		}

		private void RunDeleteTimer(ulong threadId)
		{
			var timer = new Timer();

			timer.Interval = 1000 * 60 * 60 * 24 * 2;
			timer.AutoReset = false;
			timer.Elapsed += (_, _) =>
			{
				TryRemoveRaid(threadId);
				_timersToDeleteRaid.Remove(timer);
				UpdateBackup();
				timer.Dispose();
			};
			timer.Start();

			_timersToDeleteRaid.Add(timer);
		}

		private void UpdateBackup()
		{
			_backup.Raids = ActiveRaids;
			_backup.Save();
		}
	}
}
