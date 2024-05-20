using DSharpPlus.SlashCommands;
using KorgiBot.Langs;

namespace Default.Langs.AmericanEnglish
{
	[Lang(Localization.AmericanEnglish)]
	public class EditRaidModalForm : LangBase
	{
		public EditRaidModalForm()
		{
			this[TranslationKeys.EditRaidModalFormEditTitle] = "Edit gathering";
			this[TranslationKeys.EditRaidModalFormMembersTitle] = "Changes in members list";
		}
	}
}
