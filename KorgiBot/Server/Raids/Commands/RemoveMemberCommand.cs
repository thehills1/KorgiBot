using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace KorgiBot.Server.Raids.Commands
{
	public class RemoveMemberCommand : ICommand
	{
		private readonly Bot _bot;
		private readonly RaidsManager _raidsManager;

		public RemoveMemberCommand(Bot bot, RaidsManager raidsManager)
		{
			_bot = bot;
			_raidsManager = raidsManager;
		}

		public bool TryParse(MessageCreateEventArgs args, out CommandContext context)
		{
			context = new CommandContext();

			var command = args.Message.Content;
			if (!Regex.IsMatch(command, @"^\-[0-9]{1,}$")) return false;

			command = command.Substring(1);

			context.Sender = args.Author as DiscordMember;
			context.Thread = args.Channel;
			context.Arguments = new[] { command };

			return true;
		}

		public bool TryExecute(CommandContext context)
		{
			return _raidsManager.TryRemoveMember(context.Thread, context.Sender, int.Parse(context.Arguments[0]));
		}
	}
}
