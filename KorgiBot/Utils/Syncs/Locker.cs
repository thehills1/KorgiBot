namespace KorgiBot.Utils.Syncs
{
	public class Locker : ILocker
	{
		private readonly ISyncManager _syncManager;
		private readonly string _key;

		internal Locker(ISyncManager syncManager, string key)
		{
			_syncManager = syncManager;
			_key = key;

			_syncManager.EnterSync(key);
		}

		public void Dispose()
		{
			_syncManager.TryExit(_key);
		}
	}
}