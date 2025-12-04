using System;
using System.Linq;
using KorgiBot.Database;
using KorgiBot.Utils;
using Microsoft.EntityFrameworkCore;

namespace KorgiBot
{
	public class TasksSchedule
	{
		private const int CheckRaidsToDeleteInterval = 1000 * 60 * 60;

		private readonly DatabaseContext _databaseContext;

		public TasksSchedule(DatabaseContext databaseContext)
		{
			_databaseContext = databaseContext;
		}

		public void Initialize()
		{
			TimerUtils.CreateAndStart(
				CheckRaidsToDeleteInterval, 
				() => _databaseContext.Raids
						.Where(r => r.RemoveDate < DateTimeOffset.UtcNow)
						.ExecuteDeleteAsync());
		}
	}
}