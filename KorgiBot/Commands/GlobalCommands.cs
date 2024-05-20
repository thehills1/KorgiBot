using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using KorgiBot.Commands.Autocomplete;

namespace KorgiBot.Commands
{
	[SlashModuleLifespan(SlashModuleLifespan.Singleton)]
	public class GlobalCommands : ApplicationCommandModule, IGlobalCommands
    {
		private readonly ServiceManager _serviceManager;

		public GlobalCommands(ServiceManager serviceManager)
		{
			_serviceManager = serviceManager;
		}

		[SlashCommand("reg", "Создать сбор.", false)]
		[DescriptionLocalization(Localization.AmericanEnglish, "Create raid.")]
		[DescriptionLocalization(Localization.BritishEnglish, "Create raid.")]
		public async Task StartRegistration(InteractionContext context)
        {
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.StartRegistration(context);
        }

		[SlashCommand("del", "Удалить сбор.", false)]
		[DescriptionLocalization(Localization.AmericanEnglish, "Remove raid.")]
		[DescriptionLocalization(Localization.BritishEnglish, "Remove raid.")]
		public async Task RemoveRegistration(InteractionContext context, 
			[Option("ThreadId", "Id ветки.", true)] [Autocomplete(typeof(RaidsAutocompleteProvider))]
			[DescriptionLocalization(Localization.AmericanEnglish, "Thread id.")]
			[DescriptionLocalization(Localization.BritishEnglish, "Thread id.")] string threadId)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.RemoveRegistration(context, threadId);
		}

		[SlashCommand("editreg", "Изменить участников сбора.", false)]
		[DescriptionLocalization(Localization.AmericanEnglish, "Edit raid's members.")]
		[DescriptionLocalization(Localization.BritishEnglish, "Edit raid's members.")]
		public async Task EditRegistration(InteractionContext context,
			[Option("ThreadId", "Id ветки.", true)][Autocomplete(typeof(RaidsAutocompleteProvider))]
			[DescriptionLocalization(Localization.AmericanEnglish, "Thread id.")]
			[DescriptionLocalization(Localization.BritishEnglish, "Thread id.")] string threadId)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.EditRegistration(context, threadId);
		}

		[SlashCommand("checkreg", "Проверить всех участников сбора на нахождение в вашем голосовом канале.", false)]
		[DescriptionLocalization(Localization.AmericanEnglish, "Check that all participants in the raid are in your voice channel.")]
		[DescriptionLocalization(Localization.BritishEnglish, "Check that all participants in the raid are in your voice channel.")]
		public async Task CheckOnPresence(InteractionContext context,
			[Option("ThreadId", "Id ветки.", true)][Autocomplete(typeof(RaidsAutocompleteProvider))]
			[DescriptionLocalization(Localization.AmericanEnglish, "Thread id.")]
			[DescriptionLocalization(Localization.BritishEnglish, "Thread id.")] string threadId)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.CheckOnPresence(context, threadId);
		}

		[SlashCommand("voicecheckreg", "Проверить участников всех голосовых каналов на регистрацию в сборе.", false)]
		[DescriptionLocalization(Localization.AmericanEnglish, "Check the participants of all voice channels for registration in the raid.")]
		[DescriptionLocalization(Localization.BritishEnglish, "Check the participants of all voice channels for registration in the raid.")]
		public async Task CheckVoicesOnRegistration(InteractionContext context,
			[Option("ThreadId", "Id ветки.", true)][Autocomplete(typeof(RaidsAutocompleteProvider))]
			[DescriptionLocalization(Localization.AmericanEnglish, "Thread id.")]
			[DescriptionLocalization(Localization.BritishEnglish, "Thread id.")] string threadId)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.CheckVoicesOnRegistration(context, threadId);
		}

		[SlashCommand("voicemovereg", "Проверить всех участников войсов и переместить к вам зарегистрированных.", false)]
		[DescriptionLocalization(Localization.AmericanEnglish, "Check all members of all voice channels and move registered ones to you.")]
		[DescriptionLocalization(Localization.BritishEnglish, "Check all members of all voice channels and move registered ones to you.")]
		public async Task CheckVoicesOnRegAndMove(InteractionContext context,
			[Option("ThreadId", "Id ветки.", true)][Autocomplete(typeof(RaidsAutocompleteProvider))]
			[DescriptionLocalization(Localization.AmericanEnglish, "Thread id.")]
			[DescriptionLocalization(Localization.BritishEnglish, "Thread id.")] string threadId,
			[Option("All", "Перемещать в ваш голосовой канал всех участников из других голосовых каналов.")]
			[DescriptionLocalization(Localization.AmericanEnglish, "Move all (include not registered) from another voice channels to your voice channel.")]
			[DescriptionLocalization(Localization.BritishEnglish, "Move all (include not registered) from another voice channels to your voice channel.")] bool all = false)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.CheckVoicesOnRegAndMove(context, threadId, all);
		}

		[SlashCommand("recover", "Восстановить список активных сборов.", false)]
		[DescriptionLocalization(Localization.AmericanEnglish, "Recover list of active raids.")]
		[DescriptionLocalization(Localization.BritishEnglish, "Recover list of active raids.")]
		public async Task Recover(InteractionContext context)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.Recover(context);
		}

		[SlashCommand("notify", "Предупредить не зашедших в голосовой канал о начале сбора.", false)]
		[DescriptionLocalization(Localization.AmericanEnglish, "Notify users that not joined a voice channel that raid starts.")]
		[DescriptionLocalization(Localization.BritishEnglish, "Notify users that not joined a voice channel that raid starts.")]
		public async Task NotifyRaidStarts(InteractionContext context,
			[Option("ThreadId", "Id ветки.", true)][Autocomplete(typeof(RaidsAutocompleteProvider))]
			[DescriptionLocalization(Localization.AmericanEnglish, "Thread id.")]
			[DescriptionLocalization(Localization.BritishEnglish, "Thread id.")] string threadId)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.NotifyRaidStarts(context, threadId);
		}

		[SlashCommand("sendmessagetoall", "Отправить всем участникам с определённой ролью сообщение.", false)]
		[DescriptionLocalization(Localization.AmericanEnglish, "Send a message to all participants with a specific role.")]
		[DescriptionLocalization(Localization.BritishEnglish, "Send a message to all participants with a specific role.")]
		public async Task SendMessageToAll(InteractionContext context,
			[Option("RecipientsRole", "Роль, владельцы которой получат сообщение", true)]
			[DescriptionLocalization(Localization.AmericanEnglish, "The role whose owners will receive the message")]
			[DescriptionLocalization(Localization.BritishEnglish, "The role whose owners will receive the message")] DiscordRole recipientsRole,
			[Option("Content", "Текст сообщения")]
			[DescriptionLocalization(Localization.AmericanEnglish, "Message content")]
			[DescriptionLocalization(Localization.BritishEnglish, "Message content")] string content)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.SendMessageToAll(context, recipientsRole, content);
		}
	}
}
