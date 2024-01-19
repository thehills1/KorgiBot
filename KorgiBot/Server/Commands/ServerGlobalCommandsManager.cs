using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using KorgiBot.Server.Database;
using KorgiBot.Server.Raids;

namespace KorgiBot.Server.Commands
{
	public class ServerGlobalCommandsManager
    {
		private readonly Bot _bot;
		private readonly ServerDatabaseManager _databaseManager;
		private readonly RaidsManager _raidManager;
		private readonly GuildManager _guildManager;

		public ServerGlobalCommandsManager(Bot bot, ServerDatabaseManager databaseManager, RaidsManager raidManager, GuildManager guildManager) 
        {
			_bot = bot;
			_databaseManager = databaseManager;
			_raidManager = raidManager;
			_guildManager = guildManager;
		}

        public bool TryStartRegistration(InteractionContext context, string description, string startTime, string rawMembers, out string message, long firstRequired = 20)
        {
            message = "";

			_raidManager.CreateRaid(context.Member.Id, context.Channel, description, startTime, rawMembers, (int) firstRequired);

            message = "Регистрация успешно создана.";
            return true;
        }

		public bool TryRemoveRegistration(InteractionContext context, ulong threadId, out string message)
		{
			message = "";

			var result = _raidManager.TryRemoveRaid(threadId);
			if (result)
			{
				message = "Регистрация успешно удалена.";
			}
			else
			{
				message = "Регистрации с номером такой ветки нет.";
			}

			return true;
		}

		public bool TryEditRegistration(InteractionContext context, ulong threadId, string membersChanges, out string message)
		{
			message = "";

			var result = _raidManager.TryAppendChanges(threadId, membersChanges);
			if (result)
			{
				message = "Изменения успешно внесены.";
			}
			else
			{
				message = "Произошла ошибка во время внесения изменений. Возможно, вы попытались удалить несуществующую роль.";
			}

			return result;
		}

		public bool TryCheckOnRegistration(InteractionContext context, ulong threadId, out string message)
		{
			message = "";

			CheckAllMembersOnRegistration(context, threadId, out var registered, out var notRegistered);

			message = GenerateMembersMessage(registered, notRegistered);
			return true;
		}

		public bool TryCheckOnRegAndMove(InteractionContext context, ulong threadId, out string message, bool all = false)
		{
			message = "";

			CheckAllMembersOnRegistration(context, threadId, out var registered, out var notRegistered);
			
			var channelToMove = context.Member.VoiceState.Channel;
			foreach (var pair in registered)
			{
				foreach (var user in pair.Value)
				{
					user.PlaceInAsync(channelToMove).Wait();
				}
			}

			if (all)
			{
				foreach (var pair in notRegistered)
				{
					foreach (var user in pair.Value)
					{
						user.PlaceInAsync(channelToMove).Wait();
					}
				}
			}

			message = GenerateMembersMessage(registered, notRegistered);
			return true;
		}

		public void CheckAllMembersOnRegistration(
			InteractionContext context,
			ulong threadId,
			out Dictionary<DiscordChannel, List<DiscordMember>> registered, 
			out Dictionary<DiscordChannel, List<DiscordMember>> notRegistered)
		{
			registered = new Dictionary<DiscordChannel, List<DiscordMember>>();
			notRegistered = new Dictionary<DiscordChannel, List<DiscordMember>>();
			var allChannels = _guildManager.GetAllChannels(context.Guild.Id).Result.Where(channel => channel.Type == ChannelType.Voice);
			var activeRaid = _raidManager.ActiveRaids.First(raid => raid.Key.Thread.Id == threadId).Value;
			foreach (var channel in allChannels)
			{
				foreach (var user in channel.Users)
				{
					if (activeRaid.IsRegistered(user))
					{
						if (!registered.TryAdd(channel, new List<DiscordMember>() { user }))
						{
							registered[channel].Add(user);
						}
					}
					else
					{
						if (!notRegistered.TryAdd(channel, new List<DiscordMember>() { user }))
						{
							notRegistered[channel].Add(user);
						}
					}
				}
			}
		}

		private string GenerateMembersMessage(Dictionary<DiscordChannel, List<DiscordMember>> registered, Dictionary<DiscordChannel, List<DiscordMember>> notRegistered)
		{
			var sb = new StringBuilder("**Список зарегистрированных на сбор участников:");
			sb.AppendLine();
			sb.AppendLine();
			
			foreach (var pair in registered)
			{
				sb.AppendLine(pair.Key.Mention);
				sb.AppendLine(string.Join("\n", pair.Value.Select(member => $"{member.Mention}")));
				sb.AppendLine();
				sb.AppendLine();
			}

			sb.AppendLine("**Список незарегистрированных на сбор участников:");

			foreach (var pair in notRegistered)
			{
				sb.AppendLine(pair.Key.Mention);
				sb.AppendLine(string.Join("\n", pair.Value.Select(member => $"{member.Mention}")));
				sb.AppendLine();
				sb.AppendLine();
			}

			return sb.ToString();
		}
	}
}
