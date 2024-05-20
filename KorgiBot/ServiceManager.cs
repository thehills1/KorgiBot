using KorgiBot.Configs;
using KorgiBot.Server;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace KorgiBot
{
    public class ServiceManager
    {
        private readonly ConcurrentDictionary<ulong, ServerService> _serverServices = new();

        private readonly IServiceProvider _serviceProvider;
        private readonly Bot _bot;
		private readonly BotConfig _botConfig;

		public ServiceManager(IServiceProvider serviceProvider, Bot bot, BotConfig botConfig)
        {
            _serviceProvider = serviceProvider;
            _bot = bot;
			_botConfig = botConfig;
		}

        public void Initialize()
        {
            _bot.Initialize();

			foreach (var serverId in _botConfig.ServersToRecoverRaids)
			{
				var serverService = GetServerService(serverId);
				serverService.RaidsManager.Recover();
			}
        }

        public ServerService GetServerService(ulong serverId)
        {
            return _serverServices.GetOrAdd(serverId, _serverId =>
            {
                var scope = _serviceProvider.CreateScope();
                var accessor = (ServerServiceAccessor)scope.ServiceProvider.GetService<IServerServiceAccessor>();

                var serverContext = scope.ServiceProvider.GetService<ServerContext>();
                serverContext.Setup(serverId);

                var serverService = scope.ServiceProvider.GetService<ServerService>();
                accessor.SetService(serverService);
                serverService.Initialize();

				if (!_botConfig.ServersToRecoverRaids.Contains(serverId))
				{
					_botConfig.ServersToRecoverRaids.Add(serverId);
					_botConfig.Save();
				}

                return serverService;
            });
        }
    }
}
