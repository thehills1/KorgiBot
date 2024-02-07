using System;
using System.IO;
using DSharpPlus;
using KorgiBot.Configs;
using KorgiBot.Langs;
using KorgiBot.Server;
using KorgiBot.Server.Commands;
using KorgiBot.Server.Database;
using KorgiBot.Server.Raids;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KorgiBot
{
	internal class Program
    {
        static void Main(string[] args)
        {
			var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<ServiceManager>();
			serviceCollection.AddSingleton<Bot>();
            serviceCollection.AddSingleton(container =>
            {
                var config = serviceCollection.BuildServiceProvider().GetService<BotConfig>();

                return new DiscordClient(new DiscordConfiguration
                {
                    Token = config.Token,
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                    MinimumLogLevel = LogLevel.Debug,
                    Intents = DiscordIntents.All
                });
            });

            serviceCollection.AddSingleton(container => BotConfig.LoadOrCreate("config.json"));

			InitializeServerServiceScope(serviceCollection);
			InitializeBotEnvironmentDirectories();

			using (var container = serviceCollection.BuildServiceProvider())
            {
				container.GetService<ServiceManager>().Initialize();

				LangManager.LoadLangs();

				CofigurateLogger();
				Console.ReadKey();
			}
        }

		private static void InitializeServerServiceScope(IServiceCollection serviceCollection)
		{
			serviceCollection.AddScoped<ServerContext>();
			serviceCollection.AddScoped<IServerServiceAccessor, ServerServiceAccessor>();
			serviceCollection.AddScoped<ServerService>();
			serviceCollection.AddScoped<ServerDatabaseConnector>();
			serviceCollection.AddScoped<ServerDatabaseManager>();

			serviceCollection.AddScoped<ServerGlobalCommands>();
			serviceCollection.AddScoped<ServerGlobalCommandsManager>();

			serviceCollection.AddScoped<RaidsManager>();

			serviceCollection.AddScoped(container =>
			{
				var serverContext = container.GetService<ServerContext>();
				return ServerConfig.LoadOrCreate(Path.Combine(serverContext.RootServerPath, "config.json"));
			});

			serviceCollection.AddScoped(container =>
			{
				var serverContext = container.GetService<ServerContext>();
				return RaidsConfig.LoadOrCreate(Path.Combine(serverContext.RootServerPath, "raids_config.json"));
			});

			serviceCollection.AddScoped(container =>
			{
				var serverContext = container.GetService<ServerContext>();
				return RaidsBackupConfig.LoadOrCreate(Path.Combine(serverContext.RootServerPath, "raids_backup_config.json"));
			});
		}

		private static void InitializeBotEnvironmentDirectories()
		{
			var directories = new string[]
			{
				BotEnvironment.ServersDirectoryPath,
				BotEnvironment.LogsDirectoryPath,
			};

			foreach (var directory in directories)
			{
				if (Directory.Exists(directory)) continue;

				Directory.CreateDirectory(directory);
			}
		}

		private static void CofigurateLogger()
		{
			var filePath = Path.Combine(BotEnvironment.LogsDirectoryPath, $"{DateTime.Now.ToString().Replace(":", "-").Replace("/", ".")}.log");
			using (var writer = new StreamWriter(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write), leaveOpen: true))
			{
				writer.AutoFlush = true;
				Console.SetOut(writer);
				Console.SetError(writer);
			}
		}
	}
}
