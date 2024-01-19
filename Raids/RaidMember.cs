using DSharpPlus;
using KorgiBot.Extensions;

namespace KorgiBot.Raids
{
	public class RaidMember
	{
		public string RoleName { get; set; }

		public ulong OwnerId { get; set; }

		public RaidMember(string roleName, ulong ownerId)
		{
			RoleName = roleName;
			OwnerId = ownerId;
		}

		public override string ToString()
		{
			return $"{RoleName}-{OwnerId.GetMention(MentionType.Username)}";
		}
	}
}
