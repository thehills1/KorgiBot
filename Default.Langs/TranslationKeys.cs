namespace Default.Langs
{
	public class TranslationKeys
	{
		/// GlobalCommands
		public const string CommandExecutedSuccessfully = "command.executed.success";
		public const string CommandExecutionError = "command.execution.error";
		public const string CommandExecutionUnexpectedError = "command.execution.unexpectederror";
		public const string CommandExecutionNoPerms = "command.execution.noperms";

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

		public const string RaidInVoiceChannelMembersList = "raid.list.invoice.members";
		public const string RaidNotInVoiceChannelMembersList = "raid.list.notinvoice.members";

		/// <summary><code> Args = { (string)RoleMention } </code></summary>
		public const string RaidNobodyWithSelectedRole = "raid.nobodywith.selectedrole";
		/// <summary><code> Args = { (int)SentMessagesCount } </code></summary>
		public const string RaidMessagesSentSuccessfully = "raid.messages.sent.successfully";

		/// <summary><code> Args = { (string)ParameterName } </code></summary>
		public const string ParameterMustBeNumberError = "errors.parameter.mustbe.number";
		/// <summary><code> Args = { (string)ParameterName } </code></summary>
		public const string ParameterMustBeGreaterThanZeroError = "errors.parameter.mustbe.greater.thanzero";
		/// <summary><code> Args = { (string)ParameterName } </code></summary>
		public const string ParameterMustBeGreaterOrEqualToZeroError = "errors.parameter.mustbe.greaterorequal.tozero";

		/// CreateRaidModalForm
		public const string CreateRaidModalFormTitle = "modalform.createraid.title";
		public const string CreateRaidModalFormDescriptionTitle = "modalform.createraid.decription.title";
		public const string CreateRaidModalFormStartTimeTitle = "modalform.createraid.starttime.title";
		public const string CreateRaidModalFormMembersTitle = "modalform.createraid.members.title";
		public const string CreateRaidModalFormFirstRequiredTitle = "modalform.createraid.firstrequired.title";

		// EditRaidModalForm
		public const string EditRaidModalFormEditTitle = "modalform.editraid.title";
		public const string EditRaidModalFormMembersTitle = "modalform.editraid.members.title";
	}
}
