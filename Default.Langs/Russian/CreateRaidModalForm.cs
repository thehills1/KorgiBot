using DSharpPlus.SlashCommands;
using KorgiBot.Langs;

namespace Default.Langs.Russian
{
	[Lang(Localization.Russian)]
	public class CreateRaidModalForm : LangBase
	{
		public CreateRaidModalForm()
		{
			this[TranslationKeys.CreateRaidModalFormTitle] = "Создание сбора";
			this[TranslationKeys.CreateRaidModalFormDescriptionTitle] = "Описание";
			this[TranslationKeys.CreateRaidModalFormStartTimeTitle] = "Время начала";
			this[TranslationKeys.CreateRaidModalFormMembersTitle] = "Список участников";
			this[TranslationKeys.CreateRaidModalFormFirstRequiredTitle] = "Минимум заполненных мест";
		}
	}
}
