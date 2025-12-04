using System;
using System.IO;
using KorgiBot.Server.Commands;
using KorgiBot.Server.Raids;
using KorgiBot.Server.Raids.Commands;

namespace KorgiBot.Server
{
	public class ServerService
    {
        public ServerGlobalCommands ServerGlobalCommands { get; }

		public RaidsManager RaidsManager { get; }

		public RaidCommandsManager RaidCommandsManager { get; }

		private readonly ServerContext _serverContext;

		public ServerService(
            ServerGlobalCommands serverGlobalCommands,
            ServerContext serverContext,
			RaidsManager raidsManager,
			RaidCommandsManager raidCommandsManager)
        {
            ServerGlobalCommands = serverGlobalCommands;
            _serverContext = serverContext;
			RaidsManager = raidsManager;
			RaidCommandsManager = raidCommandsManager;
		}

        public void Initialize()
        {
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
