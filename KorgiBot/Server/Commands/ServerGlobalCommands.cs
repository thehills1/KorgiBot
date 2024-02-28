using System.Threading.Tasks;
using Default.Langs;
using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using KorgiBot.Commands;
using KorgiBot.Configs;
using KorgiBot.Langs;
using KorgiBot.Server.Commands.ModalForms;

namespace KorgiBot.Server.Commands
{
	public class ServerGlobalCommands : BaseServerCommands, IGlobalCommands
    {
        private readonly ServerGlobalCommandsManager _commandsManager;
		private readonly ServerConfig _serverConfig;

		public ServerGlobalCommands(ServerGlobalCommandsManager commandsManager, ServerConfig serverConfig)
        {
            _commandsManager = commandsManager;
			_serverConfig = serverConfig;
		}

		public async Task StartRegistration(InteractionContext context)
		{
			var modalFormInfo = EditRaidModalForm.Create(
				TranslationKeys.EditRaidModalFormCreateTitle.Translate(_serverConfig.ServerLanguage),
				TranslationKeys.EditRaidModalFormDescriptionTitle.Translate(_serverConfig.ServerLanguage),
				TranslationKeys.EditRaidModalFormStartTimeTitle.Translate(_serverConfig.ServerLanguage),
				TranslationKeys.EditRaidModalFormMembersTitle.Translate(_serverConfig.ServerLanguage),
				TranslationKeys.EditRaidModalFormFirstRequiredTitle.Translate(_serverConfig.ServerLanguage));

			await context.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalFormInfo.Form);

			var response = await context.Client.GetInteractivity().WaitForModalAsync(modalFormInfo.CustomId);

			if (response.Result == null) return;

			var result = await _commandsManager.TryStartRegistration(context, response.Result.Values);

			await SendCommandExecutionResult(response.Result.Interaction, _serverConfig, result);
		}

		public async Task RemoveRegistration(InteractionContext context, string threadId)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryRemoveRegistration(ulong.Parse(threadId));

			await SendCommandExecutionResult(context, _serverConfig, result);
		}

		public async Task EditRegistration(InteractionContext context, string threadId, string membersChanges)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryEditRegistration(ulong.Parse(threadId), membersChanges);

			await SendCommandExecutionResult(context, _serverConfig, result);
		}

		public async Task CheckOnRegistration(InteractionContext context, string threadId)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryCheckOnRegistration(context, ulong.Parse(threadId));

			await SendCommandExecutionResult(context, _serverConfig, result);
		}

		public async Task CheckOnRegAndMove(InteractionContext context, string threadId, bool all = false)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryCheckOnRegAndMove(context, ulong.Parse(threadId), all);

			await SendCommandExecutionResult(context, _serverConfig, result);
		}

		public async Task Recover(InteractionContext context)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryRecover();

			await SendCommandExecutionResult(context, _serverConfig, result);
		}

		public async Task NotifyRaidStarts(InteractionContext context, string threadId)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryNotifyRaidStarts(context, ulong.Parse(threadId));

			await SendCommandExecutionResult(context, _serverConfig, result);
		}
	}
}
