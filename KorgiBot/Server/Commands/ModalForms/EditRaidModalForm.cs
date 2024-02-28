using DSharpPlus.Entities;
using DSharpPlus;

namespace KorgiBot.Server.Commands.ModalForms
{
	public static class EditRaidModalForm
	{
		public const string RaidDescriptionCustomId = "raid_description";
		public const string RaidStartTimeCustomId = "raid_start_time";
		public const string RaidMembersCustomId = "raid_members";
		public const string FirstRequiredCustomId = "first_required";

		private static object _createSync = new();

		public static int UniqueId = 0;

		public static ModalFormInfo Create(string formTitle, string descriptionTitle, string startTimeTitle, string membersTitle, string firstRequiredTitle)
		{
			return Create(formTitle, descriptionTitle, startTimeTitle, membersTitle, firstRequiredTitle, null, null, null);
		}

		public static ModalFormInfo Create(
			string formTitle, 
			string descriptionTitle, 
			string startTimeTitle, 
			string membersTitle, 
			string firstRequiredTitle,
			string description,
			string startTime,
			string members,
			string firstRequired = "0")
		{
			lock (_createSync)
			{
				var customId = $"edit_raid_form_{UniqueId++}";
				var form = new DiscordInteractionResponseBuilder()
					.WithCustomId(customId)
					.WithTitle(formTitle)
					.AddComponents(new TextInputComponent(descriptionTitle, RaidDescriptionCustomId, value: description))
					.AddComponents(new TextInputComponent(startTimeTitle, RaidStartTimeCustomId, value: startTime))
					.AddComponents(new TextInputComponent(membersTitle, RaidMembersCustomId, value: members, style: TextInputStyle.Paragraph))
					.AddComponents(new TextInputComponent(firstRequiredTitle, FirstRequiredCustomId, value: firstRequired));

				return new ModalFormInfo(customId, form);
			}
		}
	}
}
