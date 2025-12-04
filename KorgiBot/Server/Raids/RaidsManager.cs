using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using KorgiBot.Configs;
using KorgiBot.Database;
using KorgiBot.Database.Tables;
using KorgiBot.Extensions;
using KorgiBot.Utils.Syncs;
using Microsoft.EntityFrameworkCore;

namespace KorgiBot.Server.Raids
{
	public class RaidsManager
	{
		private readonly Bot _bot;
		private readonly DatabaseContext _databaseContext;
		private readonly RaidsConfig _raidsConfig;
		private readonly ISyncManager _syncManager;

		public RaidsManager(
			Bot bot,
			DatabaseContext databaseContext,
			RaidsConfig raidsConfig,
			ISyncManager syncManager)
		{
			_bot = bot;
			_databaseContext = databaseContext;
			_raidsConfig = raidsConfig;
			_syncManager = syncManager;
		}

		public async Task<bool> CheckRaidExistsAsync(ulong threadId)
		{
			return await TryGetRaidAsync(threadId) != null;
		}

		public Task<Raid> TryGetRaidAsync(ulong threadId)
		{
			return _databaseContext.Raids
				.Include(r => r.Roles)
				.FirstOrDefaultAsync(r => r.ThreadId == threadId);
		}

		public async Task CreateRaidAsync(ulong creatorId, DiscordChannel channel, string description, string startTime, string rawMembers, int firstRequired)
		{
			var roles = new List<RaidRole>();
			var orderNumber = 1;
			foreach (Match line in Regex.Matches(rawMembers, RaidPatterns.Role))
			{
				line.Groups.TryGetValue("roleName", out var roleNameGroup);
				line.Groups.TryGetValue("memberId", out var memberIdGroup);
				ulong? memberId = memberIdGroup.Value.IsNullOrEmpty() ? null : ulong.Parse(memberIdGroup.Value);

				roles.Add(new RaidRole
				{
					OrderNumber = orderNumber++,
					Name = roleNameGroup.Value,
					MemberId = memberId
				});
			}

			var sendMessagesResult = await SendRaidMessagesAsync(creatorId, channel, description, startTime, firstRequired, roles);
			var raid = new Raid()
			{
				Description = description,
				StartTime = startTime,
				GuildId = channel.GuildId.Value,
				ChannelId = channel.Id,
				ThreadId = sendMessagesResult.ThreadId,
				CreatorId = creatorId,
				FirstRequired = firstRequired,
				AdminRoles = _raidsConfig.AdminRoles,
				MessageIds = sendMessagesResult.MessageIds
			};
			_databaseContext.Add(raid);
			await _databaseContext.SaveChangesAsync();

			foreach (var role in roles)
			{
				role.RaidId = raid.Id;
			}

			_databaseContext.AddRange(roles);
			await _databaseContext.SaveChangesAsync();
		}

		public async Task<bool> TryUpdateRaidAsync(ulong threadId, string changes)
		{
			var raid = await TryGetRaidAsync(threadId);
			if (raid == null) return false;

			using (_syncManager.Lock(DefaultSyncs.UpdateRoles(raid.Id)))
			{
				var deletes = Regex.Matches(changes, RaidPatterns.RoleDelete).Select(match => match.Value);
				var additions = Regex.Matches(changes, RaidPatterns.Role).Select(match => match.Value);

				foreach (var delete in deletes)
				{
					var roleNumber = int.Parse(delete.Substring(1));
					TryRemoveRole(raid, roleNumber);
				}

				foreach (var addition in additions)
				{
					var split = addition.Split('-');
					var number = int.Parse(string.Join("", split[0].TakeWhile(c => c != '.')));
					var roleName = string.Join("", split[0].Skip(split[0].IndexOf('.') + 1));
					ulong? roleOwnerId = split[1] == string.Empty ? null : ulong.Parse(Regex.Match(split[1], RaidPatterns.RoleNumber).Value);

					AddOrUpdateRole(raid, roleName, roleOwnerId, number);
				}

				await _databaseContext.SaveChangesAsync();

				UpdateNumbers(raid);
				await _databaseContext.SaveChangesAsync();

				await OnMembersUpdateAsync(raid);
				return true;
			}
		}

		public async Task<bool> TryRemoveRaidAsync(ulong threadId)
		{
			var raid = await TryGetRaidAsync(threadId);
			if (raid == null) return false;

			_databaseContext.Remove(raid);
			await _databaseContext.SaveChangesAsync();

			return true;
		}

		public async Task<bool> TryAddMemberAsync(DiscordChannel thread, DiscordMember source, int number, DiscordMember target)
		{
			if (source == null) return false;

			var raid = await TryGetRaidAsync(thread.Id);
			if (raid == null) return false;

			if (!TryGetRoleByNumber(raid, number, out var role))
			{
				return false;
			}

			var previousMemberRole = raid.Roles.FirstOrDefault(r => r.MemberId == target.Id);
			using (_syncManager.Lock(DefaultSyncs.UpdateRole(raid.Id, role.Id, previousMemberRole?.Id)))
			{
				var hasPerms = CheckUserHavePerms(raid, source);
				var firstRequired = raid.FirstRequired;
				if (!hasPerms && firstRequired != 0 && number > firstRequired && raid.Roles.Take(firstRequired).Any(role => !role.Assigned))
				{
					return false;
				}
				
				if (!hasPerms && ((source == target && role.Assigned) || (source != target)))
				{
					return false;
				}

				if (role.MemberId == target.Id)
				{
					return false;
				}

				if (previousMemberRole != null)
				{
					previousMemberRole.MemberId = null;
					_databaseContext.Update(previousMemberRole);
				}

				role.MemberId = target.Id;

				_databaseContext.Update(role);
				await _databaseContext.SaveChangesAsync();

				await OnMembersUpdateAsync(raid);
				return true;
			}
		}

		public async Task<bool> TryRemoveMemberAsync(DiscordChannel thread, DiscordMember source, int number = 0)
		{
			if (source == null) return false;

			var raid = await TryGetRaidAsync(thread.Id);
			if (raid == null) return false;
			
			RaidRole role = null;
			if (number != 0)
			{
				if (!CheckUserHavePerms(raid, source))
				{
					return false;
				}

				if (!TryGetRoleByNumber(raid, number, out role))
				{
					return false;
				}
			}

			if (number == 0 && !TryGetRoleByMemberId(raid, source.Id, out role))
			{
				return false;
			}

			using (_syncManager.Lock(DefaultSyncs.UpdateRole(raid.Id, role.Id)))
			{
				if (!role.Assigned) return false;

				role.MemberId = null;

				_databaseContext.Update(role);
				await _databaseContext.SaveChangesAsync();

				await OnMembersUpdateAsync(raid);
				return true;
			}
		}

		private async Task<SendRaidMessagesResult> SendRaidMessagesAsync(ulong creatorId, DiscordChannel channel, string description, string startTime, int firstRequired, List<RaidRole> roles)
		{
			var messages = GetPreparedMessagesToSend(creatorId, description, startTime, firstRequired, roles);
			var sentMessages = new List<DiscordMessage>()
			{
				await _bot.SendMessageAsync(channel, messages.First())
			};

			var thread = await sentMessages.First().CreateThreadAsync(startTime, AutoArchiveDuration.Day);
			foreach (var message in messages.Skip(1))
			{
				sentMessages.Add(await _bot.SendMessageAsync(thread, message));
			}

			return new SendRaidMessagesResult
			{
				ThreadId = thread.Id,
				MessageIds = [.. sentMessages.Select(m => m.Id)]
			};
		}

		private async Task OnMembersUpdateAsync(Raid raid)
		{
			var messagesToUpdate = GetPreparedMessagesToSend(raid.CreatorId, raid.Description, raid.StartTime, raid.FirstRequired, raid.Roles);
			messagesToUpdate[0] = $"Id: {raid.ThreadId}\n\n{messagesToUpdate[0]}";

			for (int i = 0; i < raid.MessageIds.Count; i++)
			{
				var channelId = i == 0 ? raid.ChannelId : raid.ThreadId;
				await _bot.EditMessageAsync(channelId, raid.MessageIds[i], messagesToUpdate[i]);
			}

			if (messagesToUpdate.Count == raid.MessageIds.Count)
			{
				return;
			}

			if (messagesToUpdate.Count > raid.MessageIds.Count)
			{
				for (int i = raid.MessageIds.Count; i < messagesToUpdate.Count; i++)
				{
					var message = await _bot.SendMessageAsync(raid.ThreadId, messagesToUpdate[i]);
					raid.MessageIds.Add(message.Id);
				}

				return;
			}

			var messagesToDeleteCount = raid.MessageIds.Count - messagesToUpdate.Count;
			var messagesToDeleteIds = raid.MessageIds.GetRange(raid.MessageIds.Count - messagesToDeleteCount, messagesToDeleteCount);
			foreach (var messageId in messagesToDeleteIds)
			{
				await _bot.DeleteMessageAsync(raid.ThreadId, messageId);
			}
		}

		private List<string> GetPreparedMessagesToSend(ulong creatorId, string description, string startTime, int firstRequired, List<RaidRole> roles)
		{
			var messages = new List<string>();
			var messagesCount = roles.Count % 40 == 0 ? roles.Count / 40 : roles.Count / 40 + 1;
			var sb = new StringBuilder();

			sb.AppendLine($"Собирает {creatorId.GetMention(MentionType.Username)}");
			sb.AppendLine();

			if (firstRequired != 0 && roles.Count > firstRequired)
			{
				sb.AppendLine($"Нельзя записаться на роли выше {firstRequired}, пока не заполнены первые {firstRequired} ролей.");
				sb.AppendLine();
			}

			sb.AppendLine(description);
			sb.AppendLine();
			sb.AppendLine(startTime);
			sb.AppendLine();

			var membersToSend = roles;
			for (int i = 0; i < messagesCount; i++)
			{
				if (i > 0)
				{
					sb = new StringBuilder();
				}

				var toSend = membersToSend.Take(40);
				sb.Append(string.Join('\n', toSend.Take(20).Select(RoleToString)));
				if (toSend.Count() > 20)
				{
					sb.AppendLine();
					sb.AppendLine();
					sb.AppendLine();
					sb.Append(string.Join('\n', toSend.Skip(20).Take(20).Select(r => RoleToString(r))));
				}

				messages.Add(sb.ToString());
				membersToSend = [.. membersToSend.Skip(40)];
			}

			return messages;
		}

		private void AddOrUpdateRole(Raid raid, string roleName, ulong? memberId = null, int number = 0)
		{
			if (number < 0) return;

			var nextNumber = GetNextRoleNumber(raid);
			if (number == 0 || number >= nextNumber)
			{
				var newRole = new RaidRole
				{
					OrderNumber = nextNumber,
					Name = roleName,
					MemberId = memberId,
					RaidId = raid.Id
				};

				raid.Roles.Add(newRole);
				_databaseContext.Add(newRole);

				return;
			}

			if (!TryGetRoleByNumber(raid, number, out var roleToEdit))
			{
				return;
			}

			roleToEdit.Name = roleName;
			roleToEdit.MemberId = memberId;

			_databaseContext.Update(roleToEdit);
		}

		public bool TryRemoveRole(Raid raid, int number, bool update = true)
		{
			var roleToRemove = raid.Roles.FirstOrDefault(r => r.OrderNumber == number);
			if (roleToRemove == null) return false;

			_databaseContext.Remove(roleToRemove);
			raid.Roles.Remove(roleToRemove);

			if (update)
			{
				UpdateNumbers(raid);
			}

			return true;
		}

		private bool TryGetRoleByMemberId(Raid raid, ulong memberId, out RaidRole role)
		{
			role = raid.Roles.FirstOrDefault(r => r.MemberId == memberId);
			return role != null;
		}

		private bool TryGetRoleByNumber(Raid raid, int number, out RaidRole role)
		{
			role = raid.Roles.FirstOrDefault(r => r.OrderNumber == number);
			return role != null;
		}

		private int GetNextRoleNumber(Raid raid)
		{
			return raid.Roles.LastOrDefault()?.OrderNumber + 1 ?? 1;
		}

		private void UpdateNumbers(Raid raid)
		{
			var roles = raid.Roles.OrderBy(r => r.Id);
			for (int i = 0; i < roles.Count(); i++)
			{
				raid.Roles[i].OrderNumber = i + 1;
			}

			_databaseContext.UpdateRange(roles);
		}

		private bool CheckUserHavePerms(Raid raid, DiscordMember user)
		{
			return raid.AdminRoles?.Count > 0 && user.Roles.Any(role => raid.AdminRoles.Contains(role.Id));
		}

		private string RoleToString(RaidRole role)
		{
			return $"{role.OrderNumber}.{role.Name}-{role.MemberId.GetValueOrDefault().GetMention(MentionType.Username)}";
		}

		private class SendRaidMessagesResult
		{
			public ulong ThreadId { get; set; }

			public List<ulong> MessageIds { get; set; }
		}
	}
}