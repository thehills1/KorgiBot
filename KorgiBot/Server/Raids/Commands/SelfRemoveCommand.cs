using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace KorgiBot.Server.Raids.Commands
{
	public class SelfRemoveCommand : ICommand
	{
		private readonly Bot _bot;
		private readonly RaidsManager _raidsManager;

		public SelfRemoveCommand(Bot bot, RaidsManager raidsManager)
		{
			_bot = bot;
			_raidsManager = raidsManager;
		}

		public bool TryParse(MessageCreateEventArgs args, out CommandContext context)
		{
			context = new CommandContext();

			var command = args.Message.Content;
			if (command != "-") return false;

			context.Sender = args.Author as DiscordMember;
			context.Thread = args.Channel;

			return true;
		}

		public bool TryExecute(CommandContext context)
		{
			return _raidsManager.TryRemoveMember(context.Thread, context.Sender);
		}
	}
}
