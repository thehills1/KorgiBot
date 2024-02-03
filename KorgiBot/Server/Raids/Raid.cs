using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus.Entities;
using KorgiBot.Configs;
using KorgiBot.Extensions;
using Newtonsoft.Json;

namespace KorgiBot.Server.Raids
{
	public class Raid
	{
		[JsonProperty("Config")]
		private readonly RaidsConfig _config;

		[JsonProperty("Roles")]
		private readonly List<RaidRole> _roles;

		[JsonProperty("FirstRequired")]
		private readonly int _firstRequired;

		[JsonProperty("CreatorId")]
		private readonly ulong _creatorId;

		[JsonProperty]
		public string Description { get; }

		[JsonProperty]
		public string StartTime { get; }

		[JsonProperty("MembersUpdate")]
		private object _membersUpdate = new();

		[JsonIgnore]
		public List<ulong> RegisteredMembers => _roles.Select(role => role.MemberId).Where(id => id != 0).ToList();

		public Raid(RaidsConfig config, ulong creatorId, string description, string startTime, List<RaidRole> roles, int firstRequired = 20)
		{
			_creatorId = creatorId;
			_config = config;
			Description = description;
			StartTime = startTime;
			_roles = roles;
			_firstRequired = firstRequired;
		}

		public bool TryAddMember(DiscordMember source, int number, DiscordMember target = null)
		{
			lock (_membersUpdate)
			{
				if (source == null) return false;
				if (!TryGetRole(number, out var role)) return false;

				var hasPerms = (_config.AdminRoles?.Any() ?? false) && source.Roles.Select(role => role.Id).ContainsAny(_config.AdminRoles);
				if (!hasPerms && number > _firstRequired && _roles.Take(_firstRequired).Any(member => member.MemberId == 0)) return false;

				ulong memberId;
				if (target == null)
				{
					if (!hasPerms && role.MemberId != 0) return false;

					memberId = source.Id;
				}
				else
				{
					if (!hasPerms) return false;

					memberId = target.Id;
				}

				if (role.MemberId == memberId) return false;

				role.MemberId = memberId;
				RemoveMemberFromOldPlace(number, memberId);

				return true;
			}		
		}

		public bool TryRemoveMember(DiscordMember source, int number = 0)
		{
			lock (_membersUpdate)
			{
				if (source == null) return false;

				RaidRole role;
				if (number == 0)
				{
					TryGetRole(source.Id, out role);
				}
				else
				{
					if ((_config.AdminRoles?.Any() ?? false) && !source.Roles.Select(role => role.Id).ContainsAny(_config.AdminRoles)) return false;

					TryGetRole(number, out role);
				}

				if (role == null) return false;
				if (role.MemberId == 0) return false;

				role.MemberId = 0;

				return true;
			}
		}

		public void AddOrUpdateRole(string roleName, ulong userId = 0, int number = 0)
		{
			lock (_membersUpdate)
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
					roleToEdit.Name = roleName;
					roleToEdit.MemberId = userId;
				}
			}
		}

		public bool TryRemoveRole(int number, bool update = true)
		{
			lock (_membersUpdate)
			{
				if (!TryGetRole(number, out var roleToRemove)) return false;

				_roles.Remove(roleToRemove);

				if (update) UpdateNumbers();

				return true;
			}
		}

		public bool IsRegistered(DiscordMember user)
		{
			return _roles.Any(member => member.MemberId == user.Id);
		}

		public void UpdateNumbers()
		{
			var initialNumber = 1;
			foreach (var member in _roles)
			{
				member.Number = initialNumber++;
			}
		}

		public List<string> GetString()
		{
			var messages = new List<string>();
			var messagesCount = _roles.Count % 40 == 0 ? _roles.Count / 40 : _roles.Count / 40 + 1;
			var sb = new StringBuilder();

			sb.AppendLine($"Собирает {_creatorId.GetMention(DSharpPlus.MentionType.Username)}");
			sb.AppendLine();

			if (_roles.Count > _firstRequired)
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

		private void RemoveMemberFromOldPlace(int newNumber, ulong id)
		{
			for (int i = 1; i <= _roles.Count; i++)
			{
				if (i == newNumber) continue;

				if (_roles[i - 1].MemberId == id)
				{
					_roles[i - 1].MemberId = 0;
				}
			}
		}

		private bool TryGetRole(ulong memberId, out RaidRole role)
		{
			role = null;

			foreach (var member in _roles)
			{
				if (member.MemberId != memberId) continue;

				role = member;
				return true;
			}

			return false;
		}

		private bool TryGetRole(int number, out RaidRole role)
		{
			role = null;

			foreach (var member in _roles)
			{
				if (member.Number != number) continue;

				role = member;
				return true;
			}

			return false;
		}

		private int GetNextRoleNumber()
		{
			lock (_membersUpdate)
			{
				return _roles.Last()?.Number + 1 ?? 1;
			}
		}
	}
}
