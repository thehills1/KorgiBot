﻿using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

		public Task<CommandParseResult> TryParse(MessageCreateEventArgs args)
		{
			var command = args.Message.Content;
			if (!Regex.IsMatch(command, @"^\-[0-9]{1,}$")) return Task.FromResult(new CommandParseResult(false, null));

			command = command.Substring(1);

			var context = new CommandContext();
			context.Sender = args.Author as DiscordMember;
			context.Thread = args.Channel;
			context.Arguments = new[] { command };

			return Task.FromResult(new CommandParseResult(true, context));
		}

		public Task<bool> TryExecute(CommandContext context)
		{
			return _raidsManager.TryRemoveMember(context.Thread, context.Sender, int.Parse(context.Arguments[0]));
		}
	}
}
