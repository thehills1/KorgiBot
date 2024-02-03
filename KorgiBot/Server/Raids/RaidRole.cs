using DSharpPlus;
using KorgiBot.Extensions;

namespace KorgiBot.Server.Raids
{
	public class RaidRole
	{
		public int Number { get; set; }

		public string Name { get; set; }

		public ulong MemberId { get; set; }

		public RaidRole(int number, string name, ulong memberId)
		{
			Number = number;
			Name = name;
			MemberId = memberId;
		}

		public override string ToString()
		{
			return $"{Number}.{Name}-{MemberId.GetMention(MentionType.Username)}";
		}
	}
}
