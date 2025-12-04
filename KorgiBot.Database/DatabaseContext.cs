using System;
using KorgiBot.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace KorgiBot.Database
{
	public class DatabaseContext : DbContext
	{
		public DbSet<Raid> Raids { get; set; }

		public DbSet<RaidRole> RaidRoles { get; set; }

		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				foreach (var property in entityType.GetProperties())
				{
					if (property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTimeOffset?))
					{
						property.SetColumnType("timestamp with time zone");
					}
				}
			}
		}
	}
}