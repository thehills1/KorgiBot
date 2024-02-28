using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using KorgiBot.Extensions;
using Newtonsoft.Json;

namespace KorgiBot.Server.Raids
{
	public class Raid
	{
		[JsonProperty]
		public string Description { get; }

		[JsonProperty]
		public string StartTime { get; }

		[JsonIgnore]
		public List<RaidRole> AssignedRoles => _roles.Where(role => role.Assigned).ToList();

		[JsonProperty("CreatorId")]
		private readonly ulong _creatorId;

		[JsonProperty("Roles")]
		private readonly List<RaidRole> _roles;

		[JsonProperty("FirstRequired")]
		private readonly int _firstRequired;

		[JsonProperty("AdminRoles")]
		private readonly List<ulong> _adminRoles;

		[JsonProperty("RolesUpdateSync")]
		private readonly object _rolesUpdateSync = new();

		public Raid(string description, string startTime, ulong creatorId, List<RaidRole> roles, int firstRequired, List<ulong> adminRoles)
		{
			Description = description;
			StartTime = startTime;
			_creatorId = creatorId;
			_roles = roles;
			_firstRequired = firstRequired;
			_adminRoles = adminRoles;
		}

		public bool TryAddMember(DiscordMember source, int number, DiscordMember target = null)
		{
			lock (_rolesUpdateSync)
			{
				if (source == null) return false;
				if (!TryGetRole(number, out var role)) return false;

				var hasPerms = CheckUserHavePerms(source);
				if (!hasPerms && _firstRequired != 0 && number > _firstRequired && _roles.Take(_firstRequired).Any(role => !role.Assigned)) return false;

				ulong memberId;
				if (target == null)
				{
					if (!hasPerms && role.Assigned) return false;

					memberId = source.Id;
				}
				else
				{
					if (!hasPerms) return false;

					memberId = target.Id;
				}

				if (role.MemberId == memberId) return false;

				role.SetMemberId(memberId);
				RemoveMemberFromOldPlace(number, memberId);

				return true;
			}		
		}

		public bool TryRemoveMember(DiscordMember source, int number = 0)
		{
			lock (_rolesUpdateSync)
			{
				if (source == null) return false;

				RaidRole role;
				if (number == 0)
				{
					TryGetRole(source.Id, out role);
				}
				else
				{
					if (!CheckUserHavePerms(source)) return false;

					TryGetRole(number, out role);
				}

				if (role == null) return false;
				if (!role.Assigned) return false;

				role.SetMemberId(0);

				return true;
			}
		}

		public void AddOrUpdateRole(string roleName, ulong userId = 0, int number = 0)
		{
			lock (_rolesUpdateSync)
			{
				if (number < 0) return;

				var nextNumber = GetNextRoleNumber();
				if (number == 0 || number >= nextNumber)
				{
					_roles.Add(new RaidRole(nextNumber, roleName, userId));
				}
				else
				{
					TryGetRole(number, out var roleToEdit);
					roleToEdit.SetName(roleName);
					roleToEdit.SetMemberId(userId);
				}
			}
		}

		public bool TryRemoveRole(int number, bool update = true)
		{
			lock (_rolesUpdateSync)
			{
				if (!TryGetRole(number, out var roleToRemove)) return false;

				_roles.Remove(roleToRemove);

				if (update) UpdateNumbers();

				return true;
			}
		}

		public bool IsRegistered(DiscordMember user)
		{
			return _roles.Any(role => role.MemberId == user.Id);
		}

		public void UpdateNumbers()
		{
			var initialNumber = 1;
			foreach (var role in _roles)
			{
				role.SetNumber(initialNumber++);
			}
		}

		public List<string> GetPreparedMessagesToSend()
		{
			var messages = new List<string>();
			var messagesCount = _roles.Count % 40 == 0 ? _roles.Count / 40 : _roles.Count / 40 + 1;
			var sb = new StringBuilder();

			sb.AppendLine($"Собирает {_creatorId.GetMention(MentionType.Username)}");
			sb.AppendLine();

			if (_firstRequired != 0 && _roles.Count > _firstRequired)
			{
				sb.AppendLine($"Нельзя записаться на роли выше {_firstRequired}, пока не заполнены первые {_firstRequired} ролей.");
				sb.AppendLine();
			}

			sb.AppendLine(Description);
			sb.AppendLine();
			sb.AppendLine(StartTime);
			sb.AppendLine();

			var membersToSend = _roles.ToList();
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

		private bool CheckUserHavePerms(DiscordMember user)
		{
			return (_adminRoles?.Any() ?? false) && user.Roles.Select(role => role.Id).ContainsAny(_adminRoles);
		}

		private void RemoveMemberFromOldPlace(int newNumber, ulong id)
		{
			for (int i = 1; i <= _roles.Count; i++)
			{
				if (i == newNumber) continue;

				if (_roles[i - 1].MemberId == id)
				{
					_roles[i - 1].SetMemberId(0);
				}
			}
		}

		private bool TryGetRole(ulong memberId, out RaidRole output)
		{
			output = null;

			foreach (var role in _roles)
			{
				if (role.MemberId != memberId) continue;

				output = role;
				return true;
			}

			return false;
		}

		private bool TryGetRole(int number, out RaidRole output)
		{
			output = null;

			foreach (var role in _roles)
			{
				if (role.Number != number) continue;

				output = role;
				return true;
			}

			return false;
		}

		private int GetNextRoleNumber()
		{
			lock (_rolesUpdateSync)
			{
				return _roles.Last()?.Number + 1 ?? 1;
			}
		}
	}
}
