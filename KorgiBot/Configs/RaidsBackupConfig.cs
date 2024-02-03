using System.Collections.Concurrent;
using System.Collections.Generic;
using KorgiBot.Server.Raids;
using Newtonsoft.Json;

namespace KorgiBot.Configs
{
	public class RaidsBackupConfig : BaseConfig<RaidsBackupConfig>
	{
		public ConcurrentDictionary<ulong, RaidProviderConfig> Raids { get; set; }

		static RaidsBackupConfig()
		{
			CustomSettings = new JsonSerializerSettings();
			CustomSettings.Formatting = Formatting.Indented;
			CustomSettings.NullValueHandling = NullValueHandling.Ignore;
			CustomSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
		}
	}

	public class RaidProviderConfig
	{
		public RaidInfoConfig RaidInfo { get; set; }

		public Raid Raid { get; set; }
	}

	public class RaidInfoConfig
	{
		public ulong Channel { get; set; }

		public ulong Thread { get; set; }

		public List<ulong> Messages { get; set; }
	}
}
