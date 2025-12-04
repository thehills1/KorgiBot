using System;
using System.Timers;

namespace KorgiBot.Utils
{
	public static class TimerUtils
	{
		public static Timer CreateAndStart(int interval, Action elapsed)
		{
			var timer = new Timer();
			ConfigureAndStart(timer, interval, elapsed);

			return timer;
		}

		public static void ConfigureAndStart(Timer timer, int interval, Action elapsed)
		{
			timer.AutoReset = true;
			timer.Interval = interval;
			timer.Elapsed += (_, _) => elapsed();
			timer.Start();
		}
	}
}