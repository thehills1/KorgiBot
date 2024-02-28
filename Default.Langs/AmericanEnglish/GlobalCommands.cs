using DSharpPlus.SlashCommands;
using KorgiBot.Langs;

namespace Default.Langs.AmericanEnglish
{
	[Lang(Localization.AmericanEnglish)]
	public class GlobalCommands : LangBase
	{
		public GlobalCommands() 
		{
			this[TranslationKeys.CommandExecutedSuccessfully]				= "Done successfully!";
			this[TranslationKeys.CommandExecutionError]						= "An error has occurred!";
			this[TranslationKeys.CommandExecutionUnexpectedError]			= "An unknown error has occurred, please contact the developer.";

			this[TranslationKeys.RaidCreatedSuccessfully]					= "The raid has been successfully created.";
			this[TranslationKeys.RaidDeletedSuccessfully]					= "Raid has been successfully deleted.";
			this[TranslationKeys.RaidNotFound]								= "There is no raid with such a thread id.";
			this[TranslationKeys.RaidEditedSuccessfully]					= "The changes have been successfully made.";
			this[TranslationKeys.RaidEditedError]							= "An error occurred while making changes. You may have tried to delete a role that does not exist.";
			this[TranslationKeys.RaidsRecoveredSuccessfully]				= "The restoration was completed successfully.";

			this[TranslationKeys.RaidNotifyYouMustBeInVoiceChannel]			= "You must be in a voice channel to use this command.";
			this[TranslationKeys.RaidNotifyNobodyRegistered]				= "There are no registered participants in this raid.";
			this[TranslationKeys.RaidNotifyAllMembersAlreadyInVoiceChannel]	= "All participants in the raid are already in your voice channel.";
			this[TranslationKeys.RaidNotifySentSuccessfully]				= "Alerts sent successfully.";

			this[TranslationKeys.RaidNotifyYouJoinedRaid]					= "You are registered for a raid at **{0}.**";
			this[TranslationKeys.RaidNotifyYouShouldJoinVoiceChannel]		= "To take part in it, go to the voice channel {0}.";

			this[TranslationKeys.RaidRegisteredMembersList]					= "**List of participants registered for the raid:**";
			this[TranslationKeys.RaidNotRegisteredMembersList]				= "**List of participants not registered for the raid:**";

			this[TranslationKeys.ParameterMustBeNumberError]				= "Parameter [{0}] must be a number.";
			this[TranslationKeys.ParameterMustBeGreaterThanZeroError]		= "Parameter [{0}] must be greater than zero.";
			this[TranslationKeys.ParameterMustBeGreaterOrEqualToZeroError]	= "Parameter [{0}] must be greater than or equal to zero.";
		}
	}
}
