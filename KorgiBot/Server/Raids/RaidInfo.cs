using System.Collections.Generic;
using DSharpPlus.Entities;

namespace KorgiBot.Server.Raids
{
	public class RaidInfo
	{
		public DiscordChannel Channel { get; }

		public DiscordChannel Thread { get; }

		public List<DiscordMessage> Messages { get; }

		public RaidInfo(DiscordChannel channel, DiscordChannel thread, List<DiscordMessage> messages)
		{
			Channel = channel;
			Thread = thread;
			Messages = messages;
		}
	}
}
