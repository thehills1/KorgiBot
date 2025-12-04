using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KorgiBot.Database.Tables
{
	public class Raid : TableBase
	{
		private const int DaysToRemoveRaid = 7;

		public ulong GuildId { get; set; }

		/// <summary>
		/// Идентификатор канала, в котором происходит сбор.
		/// </summary>
		public ulong ChannelId { get; set; }

		/// <summary>
		/// Идентификатор обсужджения, в котором происходит сбор.
		/// </summary>
		public ulong ThreadId { get; set; }

		/// <summary>
		/// Идентификатор создателя сбора.
		/// </summary>
		public ulong CreatorId { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public string StartTime { get; set; }

		/// <summary>
		/// Количество ролей, которые должны быть заполнены, чтобы пользователь мог записаться на последующие.
		/// </summary>
		public int FirstRequired { get; set; }

		/// <summary>
		/// Роли администраторов, имеющие разрешение управлять сбором.
		/// </summary>
		public List<ulong> AdminRoles { get; set; }

		/// <summary>
		/// Список отправленных сообщений, связанных с данным сбором.
		/// </summary>
		public List<ulong> MessageIds { get; set; }

		public DateTimeOffset RemoveDate { get; set; } = DateTimeOffset.UtcNow.AddDays(DaysToRemoveRaid);

		public List<RaidRole> Roles { get; set; }

		[NotMapped]
		public List<RaidRole> AssignedRoles => Roles?.Where(role => role.Assigned).ToList();

		public bool IsRegistered(ulong memberId)
		{
			if (Roles == null) return false;

			return Roles.FirstOrDefault(r => r.MemberId == memberId) != null;
		}
	}
}