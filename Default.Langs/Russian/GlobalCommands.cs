﻿using DSharpPlus.SlashCommands;
using KorgiBot.Langs;

namespace Default.Langs.Russian
{
	[Lang(Localization.Russian)]
	public class GlobalCommands : LangBase
	{
		public GlobalCommands() 
		{
			this[TranslationKeys.CommandExecutedSuccessfully]				= "Выполнено успешно!";
			this[TranslationKeys.CommandExecutionError]						= "Произошла ошибка!";
			this[TranslationKeys.CommandExecutionUnexpectedError]			= "Произошла неизвестная ошибка, сообщите разработчику.";
			this[TranslationKeys.CommandExecutionNoPerms]					= "У вас нет прав для выполнения данной команды.";

			this[TranslationKeys.RaidCreatedSuccessfully]					= "Сбор успешно создан.";
			this[TranslationKeys.RaidDeletedSuccessfully]					= "Сбор успешно удален.";
			this[TranslationKeys.RaidNotFound]								= "Сбора с номером такой ветки нет.";
			this[TranslationKeys.RaidEditedSuccessfully]					= "Изменения успешно внесены.";
			this[TranslationKeys.RaidEditedError]							= "Произошла ошибка во время внесения изменений. Возможно, вы попытались удалить несуществующую роль.";
			this[TranslationKeys.RaidsRecoveredSuccessfully]				= "Восстановление выполнено успешно.";

			this[TranslationKeys.RaidNotifyYouMustBeInVoiceChannel]			= "Для использования этой команды вы должны находиться в голосовом канале.";
			this[TranslationKeys.RaidNotifyNobodyRegistered]				= "В данном сборе нет зарегистрированных участников.";
			this[TranslationKeys.RaidNotifyAllMembersAlreadyInVoiceChannel]	= "Все участники сбора итак находятся в вашем голосовом канале.";
			this[TranslationKeys.RaidNotifySentSuccessfully]				= "Оповещения успешно отправлены.";

			this[TranslationKeys.RaidNotifyYouJoinedRaid]					= "Вы записаны на сбор в **{0}.**";
			this[TranslationKeys.RaidNotifyYouShouldJoinVoiceChannel]		= "Для принятия участия в нём зайдите в голосовой канал {0}.";

			this[TranslationKeys.RaidRegisteredMembersList]					= "**Список зарегистрированных на сбор участников:**";
			this[TranslationKeys.RaidNotRegisteredMembersList]				= "**Список незарегистрированных на сбор участников:**";

			this[TranslationKeys.RaidInVoiceChannelMembersList]				= "**Список зарегистрированных на сбор участников, находящихся в вашем голосовом канале:**";
			this[TranslationKeys.RaidNotInVoiceChannelMembersList]			= "**Список зарегистрированных на сбор участников, не находящихся в вашем голосовом канале:**";

			this[TranslationKeys.ParameterMustBeNumberError]				= "Параметр [{0}] должен быть числом.";
			this[TranslationKeys.ParameterMustBeGreaterThanZeroError]		= "Параметр [{0}] должен быть больше нуля.";
			this[TranslationKeys.ParameterMustBeGreaterOrEqualToZeroError]	= "Параметр [{0}] должен быть больше или равен нулю.";

			this[TranslationKeys.RaidNobodyWithSelectedRole]				= "На сервере нет участников с ролью {0}.";
			this[TranslationKeys.RaidMessagesSentSuccessfully]				= "Сообщения успешно отправлены {0} участникам.";
		}
	}
}
