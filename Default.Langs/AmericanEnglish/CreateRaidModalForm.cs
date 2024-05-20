using DSharpPlus.SlashCommands;
using KorgiBot.Langs;

namespace Default.Langs.AmericanEnglish
{
	[Lang(Localization.AmericanEnglish)]
	public class CreateRaidModalForm : LangBase
	{
		public CreateRaidModalForm()
		{
			this[TranslationKeys.CreateRaidModalFormTitle] = "Create gathering";
			this[TranslationKeys.CreateRaidModalFormDescriptionTitle] = "Description";
			this[TranslationKeys.CreateRaidModalFormStartTimeTitle] = "Start time";
			this[TranslationKeys.CreateRaidModalFormMembersTitle] = "Members list";
			this[TranslationKeys.CreateRaidModalFormFirstRequiredTitle] = "Minimum number of members filled";
		}
	}
}
