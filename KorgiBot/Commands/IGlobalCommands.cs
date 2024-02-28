using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace KorgiBot.Commands
{
    public interface IGlobalCommands
    {
        Task StartRegistration(InteractionContext context);

		Task RemoveRegistration(InteractionContext context, string threadId);

		Task EditRegistration(InteractionContext context, string threadId, string membersChanges);

		Task CheckOnRegistration(InteractionContext context, string threadId);

		Task CheckOnRegAndMove(InteractionContext context, string threadId, bool all = false);

		Task Recover(InteractionContext context);

		Task NotifyRaidStarts(InteractionContext context, string threadId);
    }
}