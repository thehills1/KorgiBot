using KorgiBot.Configs;
using KorgiBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KorgiBot.Utils
{
	public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
	{
		public DatabaseContext CreateDbContext(string[] args)
		{
			var botConfig = BotConfig.Load(BotConfig.ConfigPath);
			var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
			optionsBuilder.UseNpgsql(
				botConfig.DatabaseConnectionString,
				options => options.MigrationsAssembly("KorgiBot.Database.Migrations"))
			.UseSnakeCaseNamingConvention();

			return new DatabaseContext(optionsBuilder.Options);
		}
	}
}