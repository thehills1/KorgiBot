using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace KorgiBot.Server.Raids.Commands
{
	public interface ICommand
	{
		Task<CommandParseResult> TryParse(MessageCreateEventArgs args);
		Task<bool> TryExecute(CommandContext context);
	}
}
