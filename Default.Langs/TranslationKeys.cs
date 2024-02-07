namespace Default.Langs
{
	public class TranslationKeys
	{
		public const string CommandExecutedSuccessfully = "command.executed.success";
		public const string CommandExecutionError = "command.execution.error";

		public const string RaidCreatedSuccessfully = "raid.create.success";
		public const string RaidDeletedSuccessfully = "raid.delete.success";
		public const string RaidNotFound = "raid.notfound";
		public const string RaidEditedSuccessfully = "raid.edit.success";
		public const string RaidEditedError = "raid.edit.error";
		public const string RaidsRecoveredSuccessfully = "raids.recover.success";

		public const string RaidNotifyYouMustBeInVoiceChannel = "raid.notify.youmustbeinvoicechannel";
		public const string RaidNotifyNobodyRegistered = "raid.notify.nobodyregistered";
		public const string RaidNotifyAllMembersAlreadyInVoiceChannel = "raid.notify.allalreadyinvoice";
		public const string RaidNotifySentSuccessfully = "raid.notify.sentsuccess";

		/// <summary><code> Args = { (string)StartTime } </code></summary>
		public const string RaidNotifyYouJoinedRaid = "raid.notify.youjoinedraid";
		/// <summary><code> Args = { (string)VoiceChannelMention } </code></summary>
		public const string RaidNotifyYouShouldJoinVoiceChannel = "raid.notify.youshouldjoinvoice";

		public const string RaidRegisteredMembersList = "raid.list.registeredmembers";
		public const string RaidNotRegisteredMembersList = "raid.list.notregisteredmembers";
	}
}
