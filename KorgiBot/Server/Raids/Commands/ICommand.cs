using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace KorgiBot.Server.Raids.Commands
{
	public interface ICommand
	{
		bool TryParse(MessageCreateEventArgs args, out CommandContext context);
		bool TryExecute(CommandContext context);
	}
}
