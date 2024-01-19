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

			var result = _commandsManager.TryStartRegistration(context, description, startTime, members, out var message, firstRequired);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task RemoveRegistration(InteractionContext context, string threadId)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryRemoveRegistration(context, ulong.Parse(threadId), out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task EditRegistration(InteractionContext context, string threadId, string membersChanges)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryEditRegistration(context, ulong.Parse(threadId), membersChanges, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task CheckOnRegistration(InteractionContext context, string threadId)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryCheckOnRegistration(context, ulong.Parse(threadId), out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task CheckOnRegAndMove(InteractionContext context, string threadId, bool all = false)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryCheckOnRegAndMove(context, ulong.Parse(threadId), out var message, all);

			SendCommandExecutionResult(context, result, message);
		}

		private async void SendCommandExecutionResult(InteractionContext context, bool result, string message)
		{
			if (result)
			{
				await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbedWithSuccessResult(message));
			}
			else
			{
				await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbedWithErrorResult(message));
			}
		}
	}
}
