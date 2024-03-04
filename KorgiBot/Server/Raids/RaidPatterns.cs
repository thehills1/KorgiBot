namespace KorgiBot.Server.Raids
{
	public static class RaidPatterns
	{
		public const string Role = @"[0-9]{1,}.([a-zA-Zа-яА-Я0-9]|\s){2,}-(<@[0-9]{1,}>|)";
		public const string RoleId = @"[0-9]{1,}";
		public const string RoleDelete = @"-[0-9]{1,}";
	}
}
