using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus.Entities;
using KorgiBot.Configs;
using KorgiBot.Extensions;

namespace KorgiBot.Server.Raids
{
	public class Raid
	{
		private readonly RaidsConfig _config;
		private readonly string _description;
		private readonly string _startTime;
		private readonly List<RaidMember> _members;
		private readonly int _firstRequired;

		private object _membersUpdate = new();

		public ulong CreatorId { get; }

		public Raid(RaidsConfig config, ulong creatorId, string description, string startTime, List<RaidMember> members, int firstRequired = 20)
		{
			CreatorId = creatorId;

			_config = config;
			_description = description;
			_startTime = startTime;
			_members = members;
			_firstRequired = firstRequired;
		}

		public bool TryAddMember(DiscordMember source, int number, DiscordMember target = null)
		{
			lock (_membersUpdate)
			{
				if (source == null) return false;
				if (number > _members.Count) return false;

				var hasPerms = (_config.AdminRoles?.Any() ?? true) && source.Roles.Select(role => role.Id).ContainsAny(_config.AdminRoles);
				if (!hasPerms && number > _firstRequired && _members.Take(_firstRequired).Any(member => member.OwnerId == 0)) return false;

				var memberToAdd = _members[number - 1];
				ulong memberId;
				if (target == null)
				{
					memberId = source.Id;
				}
				else
				{
					if (!hasPerms) return false;

					memberId = target.Id;
				}

				memberToAdd.OwnerId = memberId;
				RemoveMemberFromOldPlace(number, memberId);

				return true;
			}		
		}

		public bool TryRemoveMember(DiscordMember source, int number = 0)
		{
			lock (_membersUpdate)
			{
				if (source == null) return false;

				RaidMember memberToRemove;
				if (number == 0)
				{
					memberToRemove = _members.FirstOrDefault(member => member.OwnerId == source.Id);
				}
				else
				{
					if ((_config.AdminRoles?.Any() ?? false) && !source.Roles.Select(role => role.Id).ContainsAny(_config.AdminRoles)) return false;

					memberToRemove = _members[number - 1];
				}

				if (memberToRemove == null) return false;

				memberToRemove.OwnerId = 0;

				return true;
			}
		}

		public void AddRole(string roleName, ulong userId = 0, int number = 0)
		{
			lock (_membersUpdate)
			{
				if (number < 0) return;

				var nextNumber = GetNextRoleNumber();
				if (number == 0 || number >= nextNumber)
				{
					_members.Add(new RaidMember(nextNumber, roleName, userId));
				}
				else
				{
					var memberToEdit = _members.First(member => member.Number == number);
					memberToEdit.RoleName = roleName;
					memberToEdit.OwnerId = userId;
				}
			}
		}

		public bool TryRemoveRole(int number, bool update = true)
		{
			lock (_membersUpdate)
			{
				var roleToRemove = _members.FirstOrDefault(member => member.Number == number);
				if (roleToRemove == null) return false;

				_members.Remove(roleToRemove);

				if (update) UpdateNumbers();

				return true;
			}
		}

		public bool IsRegistered(DiscordMember user)
		{
			return _members.Any(member => member.OwnerId == user.Id);
		}

		public List<string> GetString()
		{
			var messages = new List<string>();
			var messagesCount = _members.Count % 40 == 0 ? _members.Count / 40 : _members.Count / 40 + 1;
			var sb = new StringBuilder();

			sb.AppendLine($"Собирает {CreatorId.GetMention(DSharpPlus.MentionType.Username)}");
			sb.AppendLine();

			if (_members.Count > _firstRequired)
			{
				sb.AppendLine($"Нельзя записаться на роли выше {_firstRequired}, пока не заполнены первые {_firstRequired} ролей.");
				sb.AppendLine();
			}

			sb.AppendLine(_description);
			sb.AppendLine();
			sb.AppendLine(_startTime);
			sb.AppendLine();

			var membersToSend = _members.ToList();
			for (int i = 0; i < messagesCount; i++)
			{	
				if (i > 0) sb = new StringBuilder();
				
				var toSend = membersToSend.Take(40);

				sb.Append(string.Join('\n', toSend.Take(20).Select(member => member.ToString())));
				if (toSend.Count() > 20)
				{
					sb.AppendLine();
					sb.AppendLine();
					sb.AppendLine();
					sb.Append(string.Join('\n', toSend.Skip(20).Take(20).Select(member => member.ToString())));
				}

				messages.Add(sb.ToString());
				membersToSend = membersToSend.Skip(40).ToList();
			}

			return messages;
		}

		private void RemoveMemberFromOldPlace(int newNumber, ulong id)
		{
			for (int i = 1; i <= _members.Count; i++)
			{
				if (i == newNumber) continue;

				if (_members[i - 1].OwnerId == id)
				{
					_members[i - 1].OwnerId = 0;
				}
			}
		}

		public void UpdateNumbers()
		{
			var initialNumber = 1;
			foreach (var member in _members)
			{
				member.Number = initialNumber++;
			}
		}

		private int GetNextRoleNumber()
		{
			lock (_membersUpdate)
			{
				return _members.Last()?.Number + 1 ?? 1;
			}
		}
	}
}
