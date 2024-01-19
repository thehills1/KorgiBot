namespace KorgiBot.Database.Tables
{
    public class RegistrationInfoTable : ITable
    {
        public ulong ChannelId { get; set; }

        public ulong SentMessageId { get; set; }

        public ulong ThreadId { get; set; }
    }
}
