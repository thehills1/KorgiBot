namespace KorgiBot.Raids
{
	public class RaidInfo
	{
		public ulong ThreadId { get; }

		public ulong MessageId { get; }

		public RaidInfo(ulong threadId, ulong messageId)
		{
			ThreadId = threadId;
			MessageId = messageId;
		}
	}
}
