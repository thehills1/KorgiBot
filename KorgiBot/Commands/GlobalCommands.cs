using System.Threading;
using System.Threading.Tasks;
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
        public async Task StartRegistration(InteractionContext context,
            [Option("Description", "Описание к сбору, а также упоминаемые роли.")] string description,
			[Option("StartTime", "Время начала сбора. Например: 17:00 UTC.")] string startTime,
			[Option("Members", "Список участников сбора (так как одна строчка, пробелы будут заменять символ следующей строки).")] string members,
			[Option("FirstRequired", "Минимум участников, который должен быть заполнен, чтобы занимать номера выше [firstRequired].")] long firstRequired = 20)
        {
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.StartRegistration(context, description, startTime, members, firstRequired);
        }

		[SlashCommand("del", "Удалить сбор.", false)]
		public async Task RemoveRegistration(InteractionContext context, 
			[Option("ThreadId", "Id ветки.", true)] [Autocomplete(typeof(RaidsAutocompleteProvider))] string threadId)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.RemoveRegistration(context, threadId);
		}

		[SlashCommand("editreg", "Изменить участников сбора.", false)]
		public async Task EditRegistration(InteractionContext context,
			[Option("ThreadId", "Id ветки.", true)][Autocomplete(typeof(RaidsAutocompleteProvider))] string threadId,
			[Option("Changes", "Изменения в списке участников")] string membersChanges)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.EditRegistration(context, threadId, membersChanges);
		}

		[SlashCommand("checkreg", "Проверить участников всех голосовых каналов на регистрацию в сборе.", false)]
		public async Task CheckOnRegistration(InteractionContext context,
			[Option("ThreadId", "Id ветки.", true)][Autocomplete(typeof(RaidsAutocompleteProvider))] string threadId)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.CheckOnRegistration(context, threadId);
		}

		[SlashCommand("movereg", "Проверить всех участников войсов и переместить к вам зарегистрированных.", false)]
		public async Task CheckOnRegAndMove(InteractionContext context,
			[Option("ThreadId", "Id ветки.", true)][Autocomplete(typeof(RaidsAutocompleteProvider))] string threadId,
			[Option("All", "Перемещать в ваш голосовой канал всех участников из других голосовых каналов.")] bool all = false)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.CheckOnRegAndMove(context, threadId, all);
		}
	}
}
