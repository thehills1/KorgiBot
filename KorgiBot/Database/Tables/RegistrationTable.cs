using Chloe.Annotations;

namespace KorgiBot.Database.Tables
{
    public class RegistrationTable : ITable
    {
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        public ulong UserId { get; set; }
    }
}
