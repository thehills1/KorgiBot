using System.Collections.Generic;

namespace KorgiBot.Server.Raids
{
	public class RaidInfo
	{
		public ulong ChannelId { get; }

		public ulong ThreadId { get; }

		public List<ulong> MessageIds { get; }

		public RaidInfo(ulong channelId, ulong threadId, List<ulong> messageIds)
		{
			ChannelId = channelId;
			ThreadId = threadId;
			MessageIds = messageIds;
		}
	}
}
