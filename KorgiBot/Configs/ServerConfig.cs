using System.Collections.Generic;
using DSharpPlus.SlashCommands;

namespace KorgiBot.Configs
{
	public class ServerConfig : BaseConfig<ServerConfig>
	{
		public Localization ServerLanguage { get; set; }

		public List<ulong> AdminRoles { get; set; }
	}
}
