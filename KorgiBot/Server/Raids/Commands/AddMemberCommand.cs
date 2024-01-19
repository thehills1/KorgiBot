using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace KorgiBot.Server.Raids.Commands
{
	public class AddMemberCommand : ICommand
	{
		private readonly Bot _bot;
		private readonly RaidsManager _raidsManager;

		public AddMemberCommand(Bot bot, RaidsManager raidsManager)
		{
			_bot = bot;
			_raidsManager = raidsManager;
		}

		public bool TryParse(MessageCreateEventArgs args, out CommandContext context)
		{
			context = new CommandContext();

			var command = args.Message.Content;
			if (!command.StartsWith("+")) return false;

			command = command.Substring(1);
			if (!Regex.IsMatch(command, @"^[0-9]{1,} \<\@[0-9]{1,}\>$")) return false;

			var split = command.Split(' ');
			var number = split[0];
			var target = _bot.GetUserInGuild(args.Guild.Id, ulong.Parse(Regex.Match(split[1], @"[0-9]{1,}").Value)).Result;
			if (target == null) return false;
			
			context.Sender = args.Author as DiscordMember;
			context.Target = target;
			context.Thread = args.Channel;
			context.Arguments = new[] { number };
			
			return true;
		}

		public bool TryExecute(CommandContext context)
		{
			return _raidsManager.TryAddMember(context.Thread, context.Sender, int.Parse(context.Arguments[0]), context.Target);
		}
	}
}
