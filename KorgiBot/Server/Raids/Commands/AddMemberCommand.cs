using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

		public async Task<CommandParseResult> TryParse(MessageCreateEventArgs args)
		{
			var command = args.Message.Content;
			if (!command.StartsWith("+")) return new CommandParseResult(false, null);

			command = command.Substring(1);
			if (!Regex.IsMatch(command, @"^[0-9]{1,} \<\@[0-9]{1,}\>$")) return new CommandParseResult(false, null);

			var split = command.Split(' ');
			var number = split[0];
			var target = await _bot.GetMemberAsync(args.Guild.Id, ulong.Parse(Regex.Match(split[1], @"[0-9]{1,}").Value));
			if (target == null) return new CommandParseResult(false, null);

			var context = new CommandContext();
			context.Sender = args.Author as DiscordMember;
			context.Target = target;
			context.Thread = args.Channel;
			context.Arguments = new[] { number };

			return new CommandParseResult(true, context);
		}

		public Task<bool> TryExecute(CommandContext context)
		{
			return _raidsManager.TryAddMember(context.Thread, context.Sender, int.Parse(context.Arguments[0]), context.Target);
		}
	}
}
