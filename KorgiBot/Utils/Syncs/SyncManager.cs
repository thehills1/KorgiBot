using System;
using System.Collections.Concurrent;
using System.Threading;

namespace KorgiBot.Utils.Syncs
{
	public class SyncManager : ISyncManager
	{
		private static readonly TimeSpan SyncTimeout = TimeSpan.FromSeconds(10);

		private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

		public Locker Lock(string key)
		{
			return new Locker(this, key);
		}

		public void EnterSync(string key)
		{
			TryEnter(key);
		}

		public bool TryExit(string key)
		{
			if (!_locks.TryGetValue(key, out var sync))
				return false;

			sync.Release();
			return true;
		}

		public bool IsEntered(string key)
		{
			return _locks.ContainsKey(key);
		}

		private bool TryEnter(string key)
		{
			var sync = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));

			try
			{
				sync.Wait(SyncTimeout);
				return true;
			}
			catch
			{
				Monitor.Exit(sync);
				throw;
			}
		}
	}
}