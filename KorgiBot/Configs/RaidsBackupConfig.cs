using System.Collections.Concurrent;
using KorgiBot.Server.Raids;
using Newtonsoft.Json;

namespace KorgiBot.Configs
{
	public class RaidsBackupConfig : BaseConfig<RaidsBackupConfig>
	{
		public ConcurrentDictionary<ulong, RaidProvider> Raids { get; set; }

		static RaidsBackupConfig()
		{
			CustomSettings = new JsonSerializerSettings();
			CustomSettings.Formatting = Formatting.Indented;
			CustomSettings.NullValueHandling = NullValueHandling.Ignore;
			CustomSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
		}
	}
}
