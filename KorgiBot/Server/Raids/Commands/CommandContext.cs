using DSharpPlus.Entities;

namespace KorgiBot.Server.Raids.Commands
{
	public class CommandContext
	{
		public DiscordMember Sender { get; set; }

		public DiscordMember Target { get; set; }

		public DiscordChannel Thread { get; set; }

		public string[] Arguments { get; set; }
	}
}
