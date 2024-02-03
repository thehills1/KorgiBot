using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using KorgiBot.Commands;
using KorgiBot.Extensions;

namespace KorgiBot.Server.Commands
{
	public class ServerGlobalCommands : IGlobalCommands
    {
        private readonly ServerGlobalCommandsManager _commandsManager;

        public ServerGlobalCommands(ServerGlobalCommandsManager commandsManager)
        {
            _commandsManager = commandsManager;
        }

		public async Task StartRegistration(InteractionContext context, string description, string startTime, string members, long firstRequired = 20)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryStartRegistration(context, description, startTime, members, firstRequired);

			await SendCommandExecutionResult(context, result);
		}

		public async Task RemoveRegistration(InteractionContext context, string threadId)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryRemoveRegistration(ulong.Parse(threadId));

			await SendCommandExecutionResult(context, result);
		}

		public async Task EditRegistration(InteractionContext context, string threadId, string membersChanges)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryEditRegistration(ulong.Parse(threadId), membersChanges);

			await SendCommandExecutionResult(context, result);
		}

		public async Task CheckOnRegistration(InteractionContext context, string threadId)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryCheckOnRegistration(context, ulong.Parse(threadId));

			await SendCommandExecutionResult(context, result);
		}

		public async Task CheckOnRegAndMove(InteractionContext context, string threadId, bool all = false)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryCheckOnRegAndMove(context, ulong.Parse(threadId), all);

			await SendCommandExecutionResult(context, result);
		}

		public async Task Recover(InteractionContext context)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryRecover();

			await SendCommandExecutionResult(context, result);
		}

		public async Task NotifyRaidStarts(InteractionContext context, string threadId)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryNotifyRaidStarts(context, ulong.Parse(threadId));

			await SendCommandExecutionResult(context, result);
		}

		private async Task SendCommandExecutionResult(InteractionContext context, CommandResult result)
		{
			if (result.Success)
			{
				await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbedWithSuccessResult(result.Message));
			}
			else
			{
				await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbedWithErrorResult(result.Message));
			}
		}	
	}
}
