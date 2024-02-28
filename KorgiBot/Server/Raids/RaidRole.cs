using System;
using DSharpPlus;
using KorgiBot.Extensions;
using Newtonsoft.Json;

namespace KorgiBot.Server.Raids
{
	public class RaidRole
	{
		[JsonProperty]
		public int Number { get; private set; }

		[JsonProperty]
		public string Name { get; private set; }

		[JsonProperty]
		public ulong MemberId { get; private set; }

		[JsonIgnore]
		public bool Assigned => MemberId != 0;

		public RaidRole(int number, string name, ulong memberId)
		{
			Number = number;
			Name = name;
			MemberId = memberId;
		}

		public void SetNumber(int number)
		{
			if (number <= 0)
			{
				throw new ArgumentException("Номер роли не может быть меньше или равен нулю.", nameof(number));
			}

			Number = number;
		}

		public void SetName(string name)
		{
			if (name.IsNullOrEmpty())
			{
				throw new ArgumentException("Имя роли не может быть null или пустым.", nameof(name));
			}

			Name = name;
		}

		public void SetMemberId(ulong memberId)
		{
			if (memberId < 0)
			{
				throw new ArgumentException("Id участника, за которым закреплена роль, не может быть меньше нуля.", nameof(memberId));
			}

			MemberId = memberId;
		}

		public override string ToString()
		{
			return $"{Number}.{Name}-{MemberId.GetMention(MentionType.Username)}";
		}
	}
}
