using Newtonsoft.Json;

namespace KorgiBot.Server.Raids
{
	public class RaidProvider
	{
		[JsonProperty]
		public RaidInfo Info { get; }

		[JsonProperty]
		public Raid Raid { get; }

		public RaidProvider(RaidInfo info, Raid raid)
		{
			Info = info;
			Raid = raid;
		}
	}
}
