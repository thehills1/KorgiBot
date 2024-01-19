using Chloe.SQLite;

namespace KorgiBot.Database
{
    public interface IDatabaseConnector
    {
        string DatabasePath { get; }
        SQLiteContext GetDBContext();
    }
}
