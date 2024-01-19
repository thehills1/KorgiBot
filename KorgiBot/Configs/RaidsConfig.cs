using System.Collections.Generic;

namespace KorgiBot.Configs
{
	public class RaidsConfig : BaseConfig<RaidsConfig>
	{
		public List<ulong> AdminRoles { get; set; }

		public string PositiveReactionName { get; set; }

		public string NegativeReactionName { get; set; }
	}
}
