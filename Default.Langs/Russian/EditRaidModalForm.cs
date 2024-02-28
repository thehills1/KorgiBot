using DSharpPlus.SlashCommands;
using KorgiBot.Langs;

namespace Default.Langs.Russian
{
	[Lang(Localization.Russian)]
	public class EditRaidModalForm : LangBase
	{
		public EditRaidModalForm()
		{
			this[TranslationKeys.EditRaidModalFormCreateTitle] = "Создание сбора";
			this[TranslationKeys.EditRaidModalFormEditTitle] = "Редактирование сбора";
			this[TranslationKeys.EditRaidModalFormDescriptionTitle] = "Описание";
			this[TranslationKeys.EditRaidModalFormStartTimeTitle] = "Время начала";
			this[TranslationKeys.EditRaidModalFormMembersTitle] = "Список участников";
			this[TranslationKeys.EditRaidModalFormFirstRequiredTitle] = "Минимум заполненных мест";
		}
	}
}
