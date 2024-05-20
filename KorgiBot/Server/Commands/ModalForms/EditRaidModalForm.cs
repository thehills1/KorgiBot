using DSharpPlus;
using DSharpPlus.Entities;

namespace KorgiBot.Server.Commands.ModalForms
{
	public static class EditRaidModalForm
	{
		public const string RaidMembersCustomId = "raid_members";

		private static object _createSync = new();
		private static int _uniqueId = 0;

		public static ModalFormInfo Create(string formTitle, string membersTitle)
		{
			lock (_createSync)
			{
				var customId = $"edit_raid_form_{_uniqueId++}";
				var form = new DiscordInteractionResponseBuilder()
					.WithCustomId(customId)
					.WithTitle(formTitle)
					.AddComponents(new TextInputComponent(membersTitle, RaidMembersCustomId, style: TextInputStyle.Paragraph));

				return new ModalFormInfo(customId, form);
			}
		}
	}
}
