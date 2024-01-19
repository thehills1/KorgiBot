using KorgiBot.Database;

namespace KorgiBot.Server.Database
{
	public sealed class ServerDatabaseManager : DatabaseManager<ServerDatabaseConnector>
    {
        public ServerDatabaseManager(ServerDatabaseConnector databaseConnector) : base(databaseConnector)
        {
        }
    }
}
