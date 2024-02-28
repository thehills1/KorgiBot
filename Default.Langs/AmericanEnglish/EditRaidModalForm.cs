using DSharpPlus.SlashCommands;
using KorgiBot.Langs;

namespace Default.Langs.AmericanEnglish
{
	[Lang(Localization.AmericanEnglish)]
	public class EditRaidModalForm : LangBase
	{
		public EditRaidModalForm()
		{
			this[TranslationKeys.EditRaidModalFormCreateTitle] = "Create raid";
			this[TranslationKeys.EditRaidModalFormEditTitle] = "Edit raid";
			this[TranslationKeys.EditRaidModalFormDescriptionTitle] = "Description";
			this[TranslationKeys.EditRaidModalFormStartTimeTitle] = "Start time";
			this[TranslationKeys.EditRaidModalFormMembersTitle] = "Members list";
			this[TranslationKeys.EditRaidModalFormFirstRequiredTitle] = "Minimum number of members filled";
		}
	}
}
