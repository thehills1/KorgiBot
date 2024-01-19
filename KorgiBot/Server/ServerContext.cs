using System;
using System.IO;

namespace KorgiBot.Server
{
    public class ServerContext
    {
        public ulong ServerId { get; private set; }

        public string RootServerPath => Path.Combine(BotEnvironment.ServersDirectoryPath, ServerId.ToString());

        private bool _isInitialized = false;

        public void Setup(ulong serverId)
        {
            if (_isInitialized) throw new Exception("ServerContext is initialized, can't change server id.");

            ServerId = serverId;

			InitializeServerDirectory();

			_isInitialized = true;
        }

		private void InitializeServerDirectory()
		{
			if (Directory.Exists(RootServerPath)) return;

			Directory.CreateDirectory(RootServerPath);
		}
    }
}
