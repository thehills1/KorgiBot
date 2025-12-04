using System;
using System.IO;
using DSharpPlus;
using KorgiBot.Configs;
using KorgiBot.Database;
using KorgiBot.Langs;
using KorgiBot.Server;
using KorgiBot.Server.Commands;
using KorgiBot.Server.Raids;
using KorgiBot.Server.Raids.Commands;
using KorgiBot.Utils.Syncs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KorgiBot
{
	public class Program
    {
        static void Main(string[] args)
        {
			var serviceCollection = new ServiceCollection();

			SetupContainer(serviceCollection);
			InitializeBotEnvironmentDirectories();

			using (var container = serviceCollection.BuildServiceProvider())
            {
				OnBeforeRun(container);

				LangManager.LoadLangs();
				CofigurateLogger();

				Console.ReadKey();
			}
        }

		private static void SetupContainer(IServiceCollection container)
		{
			container.AddSingleton<ServiceManager>();
			container.AddSingleton<Bot>();
			container.AddSingleton(c =>
			{
				var config = c.GetRequiredService<BotConfig>();
				return new DiscordClient(new DiscordConfiguration
				{
					Token = config.Token,
					TokenType = TokenType.Bot,
					AutoReconnect = true,
					MinimumLogLevel = LogLevel.Debug,
					Intents = DiscordIntents.All
				});
			});

			container.AddSingleton<ISyncManager, SyncManager>();
			container.AddSingleton<TasksSchedule>();

			container.AddScoped<ServerContext>();
			container.AddScoped<IServerServiceAccessor, ServerServiceAccessor>();
			container.AddScoped<ServerService>();

			container.AddScoped<ServerGlobalCommands>();
			container.AddScoped<ServerGlobalCommandsManager>();

			container.AddScoped<RaidsManager>();
			container.AddScoped<RaidCommandsManager>();

			container.AddSingleton(_ => BotConfig.LoadOrCreate(BotConfig.ConfigPath));
			container.AddScoped(container =>
			{
				var serverContext = container.GetService<ServerContext>();
				return ServerConfig.LoadOrCreate(Path.Combine(serverContext.RootServerPath, "config.json"));
			});

			container.AddScoped(container =>
			{
				var serverContext = container.GetService<ServerContext>();
				return RaidsConfig.LoadOrCreate(Path.Combine(serverContext.RootServerPath, "raids_config.json"));
			});

			container.AddDbContextPool<DatabaseContext>((container, options) =>
			{
				var botConfig = container.GetRequiredService<BotConfig>();
				options.UseNpgsql(
					botConfig.DatabaseConnectionString,
					assembly => assembly.MigrationsAssembly("KorgiBot.Database.Migrations"))
				.UseSnakeCaseNamingConvention();
			}, 20);
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

		private static void OnBeforeRun(IServiceProvider container)
		{
			using (var scope = container.CreateScope())
			{
				var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
				databaseContext.Database.Migrate();
			}

			var tasksSchedule = container.GetRequiredService<TasksSchedule>();
			tasksSchedule.Initialize();

			var bot = container.GetRequiredService<Bot>();
			bot.Initialize();
		}
	}
}