namespace KorgiBot.Configs
{
    public class BotConfig : BaseConfig<BotConfig>
    {
		public new const string ConfigPath = "bot_config.json";

		/// <summary>
		/// Токен бота.
		/// </summary>
		public string Token { get; set; }

		/// <summary>
		/// Список серверов, на которых при запуске необходимо восстановить активные сборы.
		/// </summary>
		public string DatabaseConnectionString { get; set; }
	}
}