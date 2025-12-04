using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KorgiBot.Database.Tables
{
	public class RaidRole : TableBase
	{
		/// <summary>
		/// Номер по порядку.
		/// </summary>
		public int OrderNumber { get; set; }

		[Required]
		public string Name { get; set; }

		/// <summary>
		/// Идентификатор пользователя, занявшего роль.
		/// </summary>
		public ulong? MemberId { get; set; }

		/// <summary>
		/// Закреплена ли роль за каким-либо пользователем.
		/// </summary>
		[NotMapped]
		public bool Assigned => MemberId.HasValue;

		public long RaidId { get; set; }

		public Raid Raid { get; set; }
	}
}