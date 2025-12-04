using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Default.Langs;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using KorgiBot.Configs;
using KorgiBot.Database.Tables;
using KorgiBot.Extensions;
using KorgiBot.Langs;
using KorgiBot.Server.Commands.ModalForms;
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

        public async Task<CommandResult> TryStartRegistration(InteractionContext context, IReadOnlyDictionary<string, string> formValues)
        {
			var description = formValues[CreateRaidModalForm.RaidDescriptionCustomId];
			var startTime = formValues[CreateRaidModalForm.RaidStartTimeCustomId];
			var members = formValues[CreateRaidModalForm.RaidMembersCustomId];
			var rawFirstRequired = formValues[CreateRaidModalForm.FirstRequiredCustomId];

			var result = CheckFirstRequired(rawFirstRequired, out var firstRequired);
			if (!result.Success) return result;

			await _raidsManager.CreateRaidAsync(context.Member.Id, context.Channel, description, startTime, members, firstRequired);
			
            return new CommandResult(true, TranslationKeys.RaidCreatedSuccessfully.Translate(_serverConfig.ServerLanguage));
        }

		public async Task<CommandResult> TryRemoveRegistration(string rawThreadId)
		{
			var checkResult = CheckThreadId(rawThreadId, out var threadId);
			if (!checkResult.Success) return checkResult;

			var result = await _raidsManager.TryRemoveRaidAsync(threadId);
			if (result)
			{
				return new CommandResult(result, TranslationKeys.RaidDeletedSuccessfully.Translate(_serverConfig.ServerLanguage));
			}
			else
			{
				return new CommandResult(result, TranslationKeys.RaidNotFound.Translate(_serverConfig.ServerLanguage));
			}
		}

		public async Task<CommandResult> TryEditRegistration(string rawThreadId, IReadOnlyDictionary<string, string> formValues)
		{
			var checkResult = CheckThreadId(rawThreadId, out var threadId);
			if (!checkResult.Success) return checkResult;

			if (!await _raidsManager.CheckRaidExistsAsync(threadId))
			{
				return new CommandResult(false, TranslationKeys.RaidNotFound.Translate(_serverConfig.ServerLanguage));
			}

			var membersChanges = formValues[EditRaidModalForm.RaidMembersCustomId];
			var result = await _raidsManager.TryUpdateRaidAsync(threadId, membersChanges);
			if (result)
			{
				return new CommandResult(result, TranslationKeys.RaidEditedSuccessfully.Translate(_serverConfig.ServerLanguage));
			}
			else
			{
				return new CommandResult(result, TranslationKeys.RaidEditedError.Translate(_serverConfig.ServerLanguage));
			}
		}

		public async Task<CommandResult> TryCheckOnPresenceAsync(InteractionContext context, string rawThreadId)
		{
			var checkResult = CheckThreadId(rawThreadId, out var threadId);
			if (!checkResult.Success) return checkResult;

			var currentRaid = await _raidsManager.TryGetRaidAsync(threadId);
			if (currentRaid == null)
			{
				return new CommandResult(false, TranslationKeys.RaidNotFound.Translate(_serverConfig.ServerLanguage));
			}

			var currentVoiceChannel = context.Member.VoiceState?.Channel;
			if (currentVoiceChannel == null)
			{
				return new CommandResult(false, TranslationKeys.RaidNotifyYouMustBeInVoiceChannel.Translate(_serverConfig.ServerLanguage));
			}

			CheckAllMembersOnPresence(currentVoiceChannel, currentRaid, out var registered, out var notRegistered);

			return new CommandResult(true, GenerateMembersMessage(registered, notRegistered));
		}

		public async Task<CommandResult> TryCheckVoicesOnRegistrationAsync(InteractionContext context, string rawThreadId)
		{
			var checkResult = CheckThreadId(rawThreadId, out var threadId);
			if (!checkResult.Success) return checkResult;

			var currentRaid = await _raidsManager.TryGetRaidAsync(threadId);
			if (currentRaid == null)
			{
				return new CommandResult(false, TranslationKeys.RaidNotFound.Translate(_serverConfig.ServerLanguage));
			}

			CheckAllVoicesOnRegistration(context, currentRaid, out var registered, out var notRegistered);

			return new CommandResult(true, GenerateMembersMessage(registered, notRegistered));
		}

		public async Task<CommandResult> TryCheckVoicesOnRegAndMove(InteractionContext context, string rawThreadId, bool all = false)
		{
			var checkResult = CheckThreadId(rawThreadId, out var threadId);
			if (!checkResult.Success) return checkResult;

			var currentRaid = await _raidsManager.TryGetRaidAsync(threadId);
			if (currentRaid == null)
			{
				return new CommandResult(false, TranslationKeys.RaidNotFound.Translate(_serverConfig.ServerLanguage));
			}

			CheckAllVoicesOnRegistration(context, currentRaid, out var registered, out var notRegistered);
			
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

		public async Task<CommandResult> TryNotifyRaidStartsAsync(InteractionContext context, string rawThreadId)
		{
			var checkResult = CheckThreadId(rawThreadId, out var threadId);
			if (!checkResult.Success) return checkResult;

			var currentRaid = await _raidsManager.TryGetRaidAsync(threadId);
			if (currentRaid == null)
			{
				return new CommandResult(false, TranslationKeys.RaidNotFound.Translate(_serverConfig.ServerLanguage));
			}

			var currentVoiceChannel = context.Member.VoiceState?.Channel;
			if (currentVoiceChannel == null)
			{
				return new CommandResult(false, TranslationKeys.RaidNotifyYouMustBeInVoiceChannel.Translate(_serverConfig.ServerLanguage));
			}

			if (currentRaid.AssignedRoles.Count == 0)
			{
				return new CommandResult(false, TranslationKeys.RaidNotifyNobodyRegistered.Translate(_serverConfig.ServerLanguage));
			}

			CheckAllMembersOnPresence(currentVoiceChannel, currentRaid, out _, out var notInVoiceChannel);

			var messageWasSent = false;
			foreach (var member in notInVoiceChannel)
			{
				var message = new StringBuilder();

				message.AppendLine(member.Mention);
				message.AppendLine();
				message.AppendLine(TranslationKeys.RaidNotifyYouJoinedRaid.Translate(_serverConfig.ServerLanguage, currentRaid.StartTime));
				message.AppendLine(TranslationKeys.RaidNotifyYouShouldJoinVoiceChannel.Translate(_serverConfig.ServerLanguage, currentVoiceChannel.Mention));

				await _bot.SendDirectMessage(member, message.ToString());

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

		public async Task<CommandResult> TrySendMessageToAll(InteractionContext context, DiscordRole recipientsRole, string content)
		{
			if (!context.Member.Roles.Select(role => role.Id).ContainsAny(_serverConfig.AdminRoles))
			{
				return new CommandResult(false, TranslationKeys.CommandExecutionNoPerms.Translate(_serverConfig.ServerLanguage));
			}

			var membersWithRole = context.Guild.Members.Where(member => member.Value.Roles.Any(role => role.Id == recipientsRole.Id)).Select(p => p.Value).ToList();
			if (membersWithRole.Count == 0)
			{
				return new CommandResult(false, TranslationKeys.RaidNobodyWithSelectedRole.Translate(_serverConfig.ServerLanguage, recipientsRole.Mention));
			}

			foreach (var member in membersWithRole)
			{
				await _bot.SendDirectMessage(member, content);
			}

			return new CommandResult(true, TranslationKeys.RaidMessagesSentSuccessfully.Translate(_serverConfig.ServerLanguage, membersWithRole.Count));
		}

		private void CheckAllMembersOnPresence(
			DiscordChannel voiceChannel,
			Raid raid, 
			out List<DiscordMember> inVoiceChannel, 
			out List<DiscordMember> notInVoiceChannel)
		{
			inVoiceChannel = new List<DiscordMember>();
			notInVoiceChannel = new List<DiscordMember>();

			foreach (var role in raid.AssignedRoles)
			{
				var member = _bot.GetMemberAsync(voiceChannel.GuildId.Value, role.MemberId.Value).Result;
				if (voiceChannel.Users.Contains(member))
				{
					inVoiceChannel.Add(member);
					continue;
				}

				notInVoiceChannel.Add(member);
			}
		}

		private void CheckAllVoicesOnRegistration(
			InteractionContext context,
			Raid raid,
			out Dictionary<DiscordChannel, List<DiscordMember>> registered, 
			out Dictionary<DiscordChannel, List<DiscordMember>> notRegistered)
		{
			registered = new Dictionary<DiscordChannel, List<DiscordMember>>();
			notRegistered = new Dictionary<DiscordChannel, List<DiscordMember>>();

			var allChannels = _bot.GetChannelsAsync(context.Guild.Id, channel => channel.Type == ChannelType.Voice).Result;
			foreach (var channel in allChannels)
			{
				foreach (var user in channel.Users)
				{
					if (raid.IsRegistered(user.Id))
					{
						if (!registered.TryAdd(channel, new List<DiscordMember>() { user }))
						{
							registered[channel].Add(user);
						}

						continue;
					}

					if (!notRegistered.TryAdd(channel, new List<DiscordMember>() { user }))
					{
						notRegistered[channel].Add(user);
					}
				}
			}
		}

		private CommandResult CheckThreadId(string rawThreadId, out ulong threadId)
		{
			if (!ulong.TryParse(rawThreadId, out threadId))
			{
				return new CommandResult(false, TranslationKeys.ParameterMustBeNumberError.Translate(_serverConfig.ServerLanguage, nameof(threadId)));
			}

			if (threadId < 0)
			{
				return new CommandResult(false, TranslationKeys.ParameterMustBeGreaterOrEqualToZeroError.Translate(_serverConfig.ServerLanguage, nameof(threadId)));
			}

			return new CommandResult(true);
		}

		private CommandResult CheckFirstRequired(string rawFirstRequired, out int firstRequired)
		{
			if (!int.TryParse(rawFirstRequired, out firstRequired))
			{
				return new CommandResult(false, TranslationKeys.ParameterMustBeNumberError.Translate(_serverConfig.ServerLanguage, nameof(firstRequired)));
			}

			if (firstRequired < 0)
			{
				return new CommandResult(false, TranslationKeys.ParameterMustBeGreaterOrEqualToZeroError.Translate(_serverConfig.ServerLanguage, nameof(firstRequired)));
			}

			return new CommandResult(true);
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

		private string GenerateMembersMessage(List<DiscordMember> inVoiceChannel, List<DiscordMember> notInVoiceChannel)
		{
			var sb = new StringBuilder();

			sb.AppendLine(TranslationKeys.RaidInVoiceChannelMembersList.Translate(_serverConfig.ServerLanguage));
			sb.AppendLine();

			foreach (var member in inVoiceChannel)
			{
				sb.AppendLine(member.Mention);
			}

			sb.AppendLine();
			sb.AppendLine(TranslationKeys.RaidNotInVoiceChannelMembersList.Translate(_serverConfig.ServerLanguage));
			sb.AppendLine();

			foreach (var member in notInVoiceChannel)
			{
				sb.AppendLine(member.Mention);
			}

			return sb.ToString();
		}
	}
}