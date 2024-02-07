using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Default.Langs;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using KorgiBot.Configs;
using KorgiBot.Langs;
using KorgiBot.Server.Raids;

namespace KorgiBot.Server.Commands
{
	public class ServerGlobalCommandsManager
    {
		private readonly Bot _bot;
		private readonly RaidsManager _raidsManager;
		private readonly ServerConfig _serverConfig;

		public ServerGlobalCommandsManager(Bot bot, RaidsManager raidManager, ServerConfig serverConfig) 
        {
			_bot = bot;
			_raidsManager = raidManager;
			_serverConfig = serverConfig;
		}

        public async Task<CommandResult> TryStartRegistration(InteractionContext context, string description, string startTime, string rawMembers, long firstRequired = 20)
        {
			await _raidsManager.CreateRaid(context.Member.Id, context.Channel, description, startTime, rawMembers, (int) firstRequired);
			
            return new CommandResult(true, TranslationKeys.RaidCreatedSuccessfully.Translate(_serverConfig.ServerLanguage));
        }

		public async Task<CommandResult> TryRemoveRegistration(ulong threadId)
		{
			var result = await _raidsManager.TryRemoveRaid(threadId);
			if (result)
			{
				return new CommandResult(result, TranslationKeys.RaidDeletedSuccessfully.Translate(_serverConfig.ServerLanguage));
			}
			else
			{
				return new CommandResult(result, TranslationKeys.RaidNotFound.Translate(_serverConfig.ServerLanguage));
			}
		}

		public async Task<CommandResult> TryEditRegistration(ulong threadId, string membersChanges)
		{
			if (!_raidsManager.RaidExists(threadId)) return new CommandResult(false, TranslationKeys.RaidNotFound.Translate(_serverConfig.ServerLanguage));

			var result = await _raidsManager.TryAppendChanges(threadId, membersChanges);
			if (result)
			{
				return new CommandResult(result, TranslationKeys.RaidEditedSuccessfully.Translate(_serverConfig.ServerLanguage));
			}
			else
			{
				return new CommandResult(result, TranslationKeys.RaidEditedError.Translate(_serverConfig.ServerLanguage));
			}
		}

		public CommandResult TryCheckOnRegistration(InteractionContext context, ulong threadId)
		{
			if (!_raidsManager.RaidExists(threadId)) return new CommandResult(false, TranslationKeys.RaidNotFound.Translate(_serverConfig.ServerLanguage));

			CheckAllMembersOnRegistration(context, threadId, out var registered, out var notRegistered);

			return new CommandResult(true, GenerateMembersMessage(registered, notRegistered));
		}

		public async Task<CommandResult> TryCheckOnRegAndMove(InteractionContext context, ulong threadId, bool all = false)
		{
			if (!_raidsManager.RaidExists(threadId)) return new CommandResult(false, TranslationKeys.RaidNotFound.Translate(_serverConfig.ServerLanguage));

			CheckAllMembersOnRegistration(context, threadId, out var registered, out var notRegistered);
			
			var channelToMove = context.Member.VoiceState.Channel;
			foreach (var pair in registered)
			{
				foreach (var user in pair.Value)
				{
					await user.PlaceInAsync(channelToMove);
				}
			}

			if (all)
			{
				foreach (var pair in notRegistered)
				{
					foreach (var user in pair.Value)
					{
						await user.PlaceInAsync(channelToMove);
					}
				}
			}

			return new CommandResult(true, GenerateMembersMessage(registered, notRegistered));
		}

		public async Task<CommandResult> TryRecover()
		{
			await _raidsManager.Recover();

			return new CommandResult(true, TranslationKeys.RaidsRecoveredSuccessfully.Translate(_serverConfig.ServerLanguage));
		}

		public async Task<CommandResult> TryNotifyRaidStarts(InteractionContext context, ulong threadId)
		{
			if (!_raidsManager.RaidExists(threadId)) return new CommandResult(false, TranslationKeys.RaidNotFound.Translate(_serverConfig.ServerLanguage));

			var currentVoiceChannel = context.Member.VoiceState?.Channel;
			if (currentVoiceChannel == null)
			{
				return new CommandResult(false, TranslationKeys.RaidNotifyYouMustBeInVoiceChannel.Translate(_serverConfig.ServerLanguage));
			}

			CheckAllMembersOnRegistration(context, threadId, out var registered, out var notRegistered);
			var currentRaid = _raidsManager.ActiveRaids[threadId];
			var membersInVoice = currentVoiceChannel.Users;

			if (!currentRaid.Raid.RegisteredMembers.Any())
			{
				return new CommandResult(false, TranslationKeys.RaidNotifyNobodyRegistered.Translate(_serverConfig.ServerLanguage));
			}

			var messageWasSent = false;
			foreach (var memberId in currentRaid.Raid.RegisteredMembers)
			{
				if (membersInVoice.Any(member => member.Id == memberId)) continue;

				var userToNotify = await _bot.GetMemberAsync(context.Guild.Id, memberId);
				var message = new StringBuilder();
				message.AppendLine(userToNotify.Mention);
				message.AppendLine();
				message.AppendLine(TranslationKeys.RaidNotifyYouJoinedRaid.Translate(_serverConfig.ServerLanguage, currentRaid.Raid.StartTime));
				message.AppendLine(TranslationKeys.RaidNotifyYouShouldJoinVoiceChannel.Translate(_serverConfig.ServerLanguage, currentVoiceChannel.Mention));

				await _bot.SendDirectMessage(userToNotify, message.ToString());
				messageWasSent = true;
			}

			if (messageWasSent)
			{
				return new CommandResult(true, TranslationKeys.RaidNotifySentSuccessfully.Translate(_serverConfig.ServerLanguage));
			}
			else
			{
				return new CommandResult(false, TranslationKeys.RaidNotifyAllMembersAlreadyInVoiceChannel.Translate(_serverConfig.ServerLanguage));
			}
		}

		public void CheckAllMembersOnRegistration(
			InteractionContext context,
			ulong threadId,
			out Dictionary<DiscordChannel, List<DiscordMember>> registered, 
			out Dictionary<DiscordChannel, List<DiscordMember>> notRegistered)
		{
			registered = new Dictionary<DiscordChannel, List<DiscordMember>>();
			notRegistered = new Dictionary<DiscordChannel, List<DiscordMember>>();
			var allChannels = _bot.GetChannelsAsync(context.Guild.Id, channel => channel.Type == ChannelType.Voice).Result;
			if (!_raidsManager.ActiveRaids.TryGetValue(threadId, out var raidProvider)) return;

			foreach (var channel in allChannels)
			{
				foreach (var user in channel.Users)
				{
					if (raidProvider.Raid.IsRegistered(user))
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
			var sb = new StringBuilder();

			sb.AppendLine(TranslationKeys.RaidRegisteredMembersList.Translate(_serverConfig.ServerLanguage));
			sb.AppendLine();
			
			foreach (var pair in registered)
			{
				sb.AppendLine(pair.Key.Mention);
				sb.AppendLine(string.Join("\n", pair.Value.Select(member => $"{member.Mention}")));
				sb.AppendLine();
				sb.AppendLine();
			}

			sb.AppendLine(TranslationKeys.RaidNotRegisteredMembersList.Translate(_serverConfig.ServerLanguage));
			sb.AppendLine();

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
