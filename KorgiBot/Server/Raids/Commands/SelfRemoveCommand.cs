using System.Threading.Tasks;
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

		public Task<CommandParseResult> TryParse(MessageCreateEventArgs args)
		{
			var command = args.Message.Content;
			if (command != "-") return Task.FromResult(new CommandParseResult(false, null));

			var context = new CommandContext();
			context.Sender = args.Author as DiscordMember;
			context.Thread = args.Channel;

			return Task.FromResult(new CommandParseResult(true, context));
		}

		public Task<bool> TryExecute(CommandContext context)
		{
			return _raidsManager.TryRemoveMember(context.Thread, context.Sender);
		}
	}
}
