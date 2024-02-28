﻿using System.Threading.Tasks;
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
			[DescriptionLocalization(Localization.BritishEnglish, "Thread id.")] string threadId,
			[Option("Changes", "Изменения в списке участников.")]
			[DescriptionLocalization(Localization.AmericanEnglish, "Changes in the members list.")]
			[DescriptionLocalization(Localization.BritishEnglish, "Changes in the members list.")] string membersChanges)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.EditRegistration(context, threadId, membersChanges);
		}

		[SlashCommand("checkreg", "Проверить участников всех голосовых каналов на регистрацию в сборе.", false)]
		public async Task CheckOnRegistration(InteractionContext context,
			[Option("ThreadId", "Id ветки.", true)][Autocomplete(typeof(RaidsAutocompleteProvider))]
			[DescriptionLocalization(Localization.AmericanEnglish, "Thread id.")]
			[DescriptionLocalization(Localization.BritishEnglish, "Thread id.")] string threadId)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.CheckOnRegistration(context, threadId);
		}

		[SlashCommand("movereg", "Проверить всех участников войсов и переместить к вам зарегистрированных.", false)]
		public async Task CheckOnRegAndMove(InteractionContext context,
			[Option("ThreadId", "Id ветки.", true)][Autocomplete(typeof(RaidsAutocompleteProvider))]
			[DescriptionLocalization(Localization.AmericanEnglish, "Thread id.")]
			[DescriptionLocalization(Localization.BritishEnglish, "Thread id.")] string threadId,
			[Option("All", "Перемещать в ваш голосовой канал всех участников из других голосовых каналов.")]
			[DescriptionLocalization(Localization.AmericanEnglish, "Move all (include not registered) from another voice channels to your voice channel.")]
			[DescriptionLocalization(Localization.BritishEnglish, "Move all (include not registered) from another voice channels to your voice channel.")] bool all = false)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.CheckOnRegAndMove(context, threadId, all);
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
	}
}
