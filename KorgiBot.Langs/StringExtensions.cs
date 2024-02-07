using DSharpPlus.SlashCommands;

namespace KorgiBot.Langs
{
	public static class StringExtension
	{
		public static string Translate(this string translationKey, Localization lang, params object[] args)
		{
			return LangManager.GetTranslation(translationKey, lang, args);
		}

#nullable enable
		public static string? TryTranslate(this string translationKey, Localization lang, params object[] args)
		{
			return LangManager.TryGetTranslation(translationKey, lang, args);
		}
#nullable restore
	}
}
