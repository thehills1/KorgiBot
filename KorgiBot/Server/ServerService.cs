using System;
using System.IO;
using KorgiBot.Server.Commands;
using KorgiBot.Server.Database;
using KorgiBot.Server.Raids;

namespace KorgiBot.Server
{
	public class ServerService
    {
        public ServerGlobalCommands ServerGlobalCommands { get; }
        public ServerDatabaseManager DatabaseManager { get; }
		public RaidsManager RaidsManager { get; }

		private readonly ServerContext _serverContext;

		public ServerService(
            ServerGlobalCommands serverGlobalCommands,
			ServerDatabaseManager databaseManager,
            ServerContext serverContext,
			RaidsManager raidsManager)
        {
            ServerGlobalCommands = serverGlobalCommands;
			DatabaseManager = databaseManager;
            _serverContext = serverContext;
			RaidsManager = raidsManager;
		}

        public void Initialize()
        {
            DatabaseManager.Initialize();
            InitializeServerDirectories();

            Console.WriteLine($"Server service for server with id: [{_serverContext.ServerId}] was initialized.");
        }

        private void InitializeServerDirectories()
        {
            var directoriesToInitialize = new string[]
            {
               
            };

            foreach (var directory in directoriesToInitialize)
            {
                if (Directory.Exists(directory)) continue;

                Directory.CreateDirectory(directory);
            }
        }
    }
}
