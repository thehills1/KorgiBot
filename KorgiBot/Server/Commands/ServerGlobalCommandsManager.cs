using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using KorgiBot.Server.Raids;

namespace KorgiBot.Server.Commands
{
	public class ServerGlobalCommandsManager
    {
		private readonly RaidsManager _raidsManager;
		private readonly Bot _bot;

		public ServerGlobalCommandsManager(RaidsManager raidManager, Bot bot) 
        {
			_raidsManager = raidManager;
			_bot = bot;
		}

        public async Task<CommandResult> TryStartRegistration(InteractionContext context, string description, string startTime, string rawMembers, long firstRequired = 20)
        {
			await _raidsManager.CreateRaid(context.Member.Id, context.Channel, description, startTime, rawMembers, (int) firstRequired);

            return new CommandResult(true, "Регистрация успешно создана.");
        }

		public async Task<CommandResult> TryRemoveRegistration(ulong threadId)
		{
			var result = await _raidsManager.TryRemoveRaid(threadId);
			if (result)
			{
				return new CommandResult(result, "Регистрация успешно удалена.");
			}
			else
			{
				return new CommandResult(result, "Регистрации с номером такой ветки нет.");
			}
		}

		public async Task<CommandResult> TryEditRegistration(ulong threadId, string membersChanges)
		{
			var result = await _raidsManager.TryAppendChanges(threadId, membersChanges);
			if (result)
			{
				return new CommandResult(result, "Изменения успешно внесены.");
			}
			else
			{
				return new CommandResult(result, "Произошла ошибка во время внесения изменений. Возможно, вы попытались удалить несуществующую роль.");
			}
		}

		public CommandResult TryCheckOnRegistration(InteractionContext context, ulong threadId)
		{
			CheckAllMembersOnRegistration(context, threadId, out var registered, out var notRegistered);

			return new CommandResult(true, GenerateMembersMessage(registered, notRegistered));
		}

		public async Task<CommandResult> TryCheckOnRegAndMove(InteractionContext context, ulong threadId, bool all = false)
		{
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

			return new CommandResult(true, "Восстановление выполнено успешно.");
		}

		public async Task<CommandResult> TryNotifyRaidStarts(InteractionContext context, ulong threadId)
		{
			var currentVoiceChannel = context.Member.VoiceState?.Channel;
			if (currentVoiceChannel == null)
			{
				return new CommandResult(false, "Для использования этой команды вы должны находиться в голосовом канале.");
			}

			CheckAllMembersOnRegistration(context, threadId, out var registered, out var notRegistered);
			var currentRaid = _raidsManager.ActiveRaids[threadId];
			var membersInVoice = currentVoiceChannel.Users;

			if (!currentRaid.Raid.RegisteredMembers.Any()) return new CommandResult(false, "В данном сборе нет зарегистрированных участников.");

			var messageWasSent = false;
			foreach (var memberId in currentRaid.Raid.RegisteredMembers)
			{
				if (membersInVoice.Any(member => member.Id == memberId)) continue;

				var userToNotify = await _bot.GetMemberAsync(context.Guild.Id, memberId);
				var message = new StringBuilder();
				message.AppendLine(userToNotify.Mention);
				message.AppendLine();
				message.AppendLine($"Вы записаны на сбор в **{currentRaid.Raid.StartTime}.**");
				message.AppendLine($"Для принятия участия в нём зайдите в голосовой канал {currentVoiceChannel.Mention}.");

				await _bot.SendDirectMessage(userToNotify, message.ToString());
				messageWasSent = true;
			}

			if (messageWasSent)
			{
				return new CommandResult(true, "Оповещения успешно отправлены.");
			}
			else
			{
				return new CommandResult(false, "Все участники сбора итак находятся в вашем голосовом канале.");
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
