using System.Threading.Tasks;
using Default.Langs;
using DSharpPlus;
using DSharpPlus.Entities;
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
			var modalFormInfo = CreateRaidModalForm.Create(
														TranslationKeys.CreateRaidModalFormTitle.Translate(_serverConfig.ServerLanguage),
														TranslationKeys.CreateRaidModalFormDescriptionTitle.Translate(_serverConfig.ServerLanguage),
														TranslationKeys.CreateRaidModalFormStartTimeTitle.Translate(_serverConfig.ServerLanguage),
														TranslationKeys.CreateRaidModalFormMembersTitle.Translate(_serverConfig.ServerLanguage),
														TranslationKeys.CreateRaidModalFormFirstRequiredTitle.Translate(_serverConfig.ServerLanguage));

			await context.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalFormInfo.Form);

			var response = await context.Client.GetInteractivity().WaitForModalAsync(modalFormInfo.CustomId);

			if (response.Result == null) return;

			var result = await _commandsManager.TryStartRegistration(context, response.Result.Values);

			await SendCommandExecutionResult(response.Result.Interaction, _serverConfig, result);
		}

		public async Task RemoveRegistration(InteractionContext context, string threadId)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryRemoveRegistration(threadId);

			await SendCommandExecutionResult(context, _serverConfig, result);
		}

		public async Task EditRegistration(InteractionContext context, string threadId)
		{
			var modalFormInfo = EditRaidModalForm.Create(
													TranslationKeys.EditRaidModalFormEditTitle.Translate(_serverConfig.ServerLanguage),
													TranslationKeys.EditRaidModalFormMembersTitle.Translate(_serverConfig.ServerLanguage));

			await context.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalFormInfo.Form);

			var response = await context.Client.GetInteractivity().WaitForModalAsync(modalFormInfo.CustomId);

			if (response.Result == null) return;

			var result = await _commandsManager.TryEditRegistration(threadId, response.Result.Values);

			await SendCommandExecutionResult(response.Result.Interaction, _serverConfig, result);
		}

		public async Task CheckOnPresence(InteractionContext context, string threadId)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryCheckOnPresence(context, threadId);

			await SendCommandExecutionResult(context, _serverConfig, result);
		}

		public async Task CheckVoicesOnRegistration(InteractionContext context, string threadId)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryCheckVoicesOnRegistration(context, threadId);

			await SendCommandExecutionResult(context, _serverConfig, result);
		}

		public async Task CheckVoicesOnRegAndMove(InteractionContext context, string threadId, bool all = false)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryCheckVoicesOnRegAndMove(context, threadId, all);

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

			var result = await _commandsManager.TryNotifyRaidStarts(context, threadId);

			await SendCommandExecutionResult(context, _serverConfig, result);
		}

		public async Task SendMessageToAll(InteractionContext context, DiscordRole recipientsRole, string content)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TrySendMessageToAll(context, recipientsRole, content);

			await SendCommandExecutionResult(context, _serverConfig, result);
		}
	}
}
