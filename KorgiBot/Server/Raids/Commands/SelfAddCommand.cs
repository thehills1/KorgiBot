using System.Linq;
using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace KorgiBot.Server.Raids.Commands
{
	public class SelfAddCommand : ICommand
	{
		private readonly Bot _bot;
		private readonly RaidsManager _raidsManager;

		public SelfAddCommand(Bot bot, RaidsManager raidsManager)
		{
			_bot = bot;
			_raidsManager = raidsManager;
		}

		public bool TryParse(MessageCreateEventArgs args, out CommandContext context)
		{
			context = new CommandContext();

			var command = args.Message.Content;
			if (!Regex.IsMatch(command, @"^[0-9]{1,}$")) return false;

			context.Sender = args.Author as DiscordMember;
			context.Thread = args.Channel;
			context.Arguments = new[] { command };

			return true;
		}

		public bool TryExecute(CommandContext context)
		{
			return _raidsManager.TryAddMember(context.Thread, context.Sender, int.Parse(context.Arguments[0]));
		}
	}
}
