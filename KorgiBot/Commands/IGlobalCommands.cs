using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace KorgiBot.Commands
{
    public interface IGlobalCommands
    {
        Task StartRegistration(InteractionContext context, string description, string startTime, string members, long firstRequired = 20);

		Task RemoveRegistration(InteractionContext context, string threadId);

		Task EditRegistration(InteractionContext context, string threadId, string membersChanges);

		Task CheckOnRegistration(InteractionContext context, string threadId);

		Task CheckOnRegAndMove(InteractionContext context, string threadId, bool all = false);
    }
}