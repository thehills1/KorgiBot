using DSharpPlus.SlashCommands;

namespace KorgiBot.Configs
{
	public class ServerConfig : BaseConfig<ServerConfig>
	{
		public Localization ServerLanguage { get; set; }
	}
}
