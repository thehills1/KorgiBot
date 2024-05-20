using System.Collections.Generic;

namespace KorgiBot.Configs
{
    public class BotConfig : BaseConfig<BotConfig>
    {
        /// <summary>
        /// Токен бота.
        /// </summary>
        public string Token { get; set; }

		/// <summary>
		/// Список серверов, на которых при запуске необходимо восстановить активные сборы.
		/// </summary>
		public List<ulong> ServersToRecoverRaids { get; set; }  
    }
}
