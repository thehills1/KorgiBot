using System;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;
using KorgiBot.Configs;
using KorgiBot.Extensions;

namespace KorgiBot.Raids
{
	public class Raid
	{
		private readonly RaidsConfig _config;
		private readonly string _description;
		private readonly string _startTime;
		private readonly List<RaidMember> _members;

		private object _membersUpdate = new();

		public ulong CreatorId { get; }

		public Raid(RaidsConfig config, ulong creatorId, string description, string startTime, List<RaidMember> members)
		{
			CreatorId = creatorId;

			_config = config;
			_description = description;
			_startTime = startTime;
			_members = members;
		}

		public event EventHandler MembersUpdate;

		public bool TryAddMember(DiscordMember source, int number, ulong ownerId)
		{
			lock (_membersUpdate)
			{
				var memberToAdd = _members[number - 1];

				if (!source.Roles?.Select(role => role.Id).ContainsAny(_config.AdminRoles) ?? true) return false;

				memberToAdd.OwnerId = ownerId;

				MembersUpdate?.Invoke(this, null);

				return true;
			}		
		}

		public override string ToString()
		{
			var membersString = "";
			for (int i = 1; i <= _members.Count; i++)
			{
				membersString += $"{i}.{_members[i-1]}\n";
			}

			return $"Собирает {CreatorId.GetMention(DSharpPlus.MentionType.Username)}\n\n{_description}\n\n{_startTime}\n\n{membersString}";
		}
	}
}
