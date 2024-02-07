using System;
using DSharpPlus.SlashCommands;

namespace KorgiBot.Langs
{
	public class LangAttribute : Attribute
	{
		public Localization Lang { get; }

		public LangAttribute(Localization lang)
		{
			Lang = lang;
		}
	}
}
