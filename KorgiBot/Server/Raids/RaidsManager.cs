using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
		private readonly GuildManager _guildManager;
		private readonly RaidsConfig _raidsConfig;
		private readonly RaidsCommandsManager _commandsManager;

		private List<Timer> _timersToDeleteRaid = new();

		public Dictionary<RaidInfo, Raid> ActiveRaids { get; } = new();

		public RaidsManager(Bot bot, GuildManager guildManager, RaidsConfig raidsConfig)
		{
			bot.MessageCreated += async (source, args) => HandleMessageCreated(args);

			_guildManager = guildManager;
			_raidsConfig = raidsConfig;
			_commandsManager = new RaidsCommandsManager(bot, this, _guildManager, _raidsConfig);

			Task.Run(() => _commandsManager.Initialize());
		}

		public void CreateRaid(ulong creatorId, DiscordChannel channel, string description, string startTime, string rawMembers, int firstRequired = 20)
		{
			var members = new List<RaidMember>();
			var number = 1;
			foreach (Match line in Regex.Matches(rawMembers, @"[0-9]{1,2}\.([a-zA-Zа-яА-Я0-9]|\s){2,}\-(\<\@[0-9]{1,}\>|)"))
			{
				var split = line.Value.Split('-');
				var roleName = string.Join("", split[0].Skip(split[0].IndexOf('.') + 1));
				ulong roleOwnerId = split[1] == string.Empty ? 0 : ulong.Parse(Regex.Match(split[1], @"[0-9]{1,}").Value);

				members.Add(new RaidMember(number++, roleName, roleOwnerId));
			}

			var raid = new Raid(_raidsConfig, creatorId, description, startTime, members, firstRequired);
			var messages = raid.GetString();
			var sentMessages = new List<DiscordMessage>() { _guildManager.SendMessageAsync(channel, messages.First()).Result };
			var thread = sentMessages.First().CreateThreadAsync(startTime, AutoArchiveDuration.Day).Result;
			sentMessages.AddRange(_guildManager.SendMessagesAsync(thread, messages.Skip(1).ToList()).Result);

			var raidInfo = new RaidInfo(channel, thread, sentMessages);
			ActiveRaids.Add(raidInfo, raid);

			OnMembersUpdate(raidInfo);

			RunDeleteTimer(thread.Id);
		}

		public bool TryAppendChanges(ulong threadId, string changes)
		{
			if (!IsRaidExists(threadId, out var raidInfo)) return false;

			var deletes = Regex.Matches(changes, @"\-[0-9]{1,}").Select(match => match.Value);
			var additions = Regex.Matches(changes, @"[0-9]{1,}\.([A-Za-zА-Яа-я0-9]|\s){2,}\-(\<\@[0-9]{1,}\>|)").Select(match => match.Value);

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
				ulong roleOwnerId = split[1] == string.Empty ? 0 : ulong.Parse(Regex.Match(split[1], @"[0-9]{1,}").Value);

				result = TryAddRole(threadId, roleName, roleOwnerId, number);
				if (!result) break;
			}

			ActiveRaids[raidInfo].UpdateNumbers();
			OnMembersUpdate(raidInfo);

			return result;
		}

		public bool TryRemoveRaid(ulong threadId)
		{
			if (!IsRaidExists(threadId, out var raidInfo)) return false;

			ActiveRaids.Remove(raidInfo);

			try
			{
				_guildManager.DeleteMessageAsync(raidInfo.Messages.First()).Wait();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while deletion raid message\n{e}");
			}

			try
			{
				raidInfo.Thread.DeleteAsync().Wait();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while deletion raid thread\n{e}");
			}			

			return true;
		}

		public bool TryAddRole(ulong threadId, string roleName, ulong userId, int number = 0)
		{
			if (!IsRaidExists(threadId, out var raidInfo)) return false;

			ActiveRaids[raidInfo].AddRole(roleName, userId, number);
			return true;
		}

		public bool TryRemoveRole(ulong threadId, int number)
		{
			if (!IsRaidExists(threadId, out var raidInfo)) return false;

			return ActiveRaids[raidInfo].TryRemoveRole(number, false);
		}

		public bool TryAddMember(DiscordChannel thread, DiscordMember source, int number, DiscordMember target = null)
		{
			if (!IsRaidExists(thread.Id, out var raidInfo)) return false;

			var result = ActiveRaids[raidInfo].TryAddMember(source, number, target);
			if (result) OnMembersUpdate(raidInfo);
			
			return result;
		}

		public bool TryRemoveMember(DiscordChannel thread, DiscordMember source, int number = 0)
		{
			if (!IsRaidExists(thread.Id, out var raidInfo)) return false;

			var result = ActiveRaids[raidInfo].TryRemoveMember(source, number);
			if (result) OnMembersUpdate(raidInfo);

			return result;
		}

		public void UpdateMembers(ulong threadId)
		{
			if (!IsRaidExists(threadId, out var raidInfo)) return;

			OnMembersUpdate(raidInfo);
		}

		private void HandleMessageCreated(MessageCreateEventArgs args)
		{
			if (!IsRaidExists(args.Channel.Id, out var raidInfo)) return;

			_commandsManager.HandleCommand(args);
		}

		private void OnMembersUpdate(RaidInfo raidInfo)
		{
			var messagesToUpdate = ActiveRaids[raidInfo].GetString();
			messagesToUpdate[0] = $"Id: {raidInfo.Thread.Id}\n\n{messagesToUpdate[0]}";
			_guildManager.EditMessagesAsync(raidInfo.Messages, messagesToUpdate).Wait();
		}

		private bool IsRaidExists(ulong threadId, out RaidInfo raidInfo)
		{
			raidInfo = ActiveRaids.Keys.FirstOrDefault(raidInfo => raidInfo.Thread.Id == threadId);
			return raidInfo != null;
		}

		private void RunDeleteTimer(ulong threadId)
		{
			var timer = new Timer();
			timer.Interval = 24 * 60 * 60 * 1000;
			timer.AutoReset = false;
			timer.Elapsed += (_, _) =>
			{
				TryRemoveRaid(threadId);
				_timersToDeleteRaid.Remove(timer);
				timer.Dispose();
			};
			timer.Start();

			_timersToDeleteRaid.Add(timer);
		}
	}
}
