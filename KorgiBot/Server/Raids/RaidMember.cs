using DSharpPlus;
using KorgiBot.Extensions;

namespace KorgiBot.Server.Raids
{
	public class RaidMember
	{
		public int Number { get; set; }

		public string RoleName { get; set; }

		public ulong OwnerId { get; set; }

		public RaidMember(int number, string roleName, ulong ownerId)
		{
			Number = number;
			RoleName = roleName;
			OwnerId = ownerId;
		}

		public override string ToString()
		{
			return $"{Number}.{RoleName}-{OwnerId.GetMention(MentionType.Username)}";
		}
	}
}
