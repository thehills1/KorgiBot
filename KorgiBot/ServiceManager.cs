using System;
using System.Collections.Generic;
using KorgiBot.Server;
using Microsoft.Extensions.DependencyInjection;

namespace KorgiBot
{
    public class ServiceManager
    {
		private static readonly HashSet<ulong> InitializedServerServices = [];

        private readonly IServiceProvider _serviceProvider;

		public ServiceManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
		}

        public ServerService GetServerService(ulong serverId)
        {
			var scope = _serviceProvider.CreateScope();
			var accessor = (ServerServiceAccessor) scope.ServiceProvider.GetService<IServerServiceAccessor>();

			var serverContext = scope.ServiceProvider.GetService<ServerContext>();
			serverContext.Setup(serverId);

			var serverService = scope.ServiceProvider.GetService<ServerService>();
			accessor.SetService(serverService);
			serverService.RaidCommandsManager.Initialize();

			if (InitializedServerServices.Contains(serverId))
			{
				return serverService;
			}

			serverService.Initialize();
			InitializedServerServices.Add(serverId);

			return serverService;
		}
	}
}