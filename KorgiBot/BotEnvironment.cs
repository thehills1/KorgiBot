using System.IO;

namespace KorgiBot
{
	public class BotEnvironment
    {
        public static string ServersDirectoryPath = "servers";
		public static string DataPath = "data";
		public static string AutocompleteProvidersChoicesPath = Path.Combine(DataPath, "autocomplete");
	}
}
