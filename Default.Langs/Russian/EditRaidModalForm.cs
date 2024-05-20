using DSharpPlus.SlashCommands;
using KorgiBot.Langs;

namespace Default.Langs.Russian
{
	[Lang(Localization.Russian)]
	public class EditRaidModalForm : LangBase
	{
		public EditRaidModalForm()
		{
			this[TranslationKeys.EditRaidModalFormEditTitle] = "Редактирование сбора";
			this[TranslationKeys.EditRaidModalFormMembersTitle] = "Изменения в списке участников";
		}
	}
}
