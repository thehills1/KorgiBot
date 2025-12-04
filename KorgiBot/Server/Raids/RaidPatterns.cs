namespace KorgiBot.Server.Raids
{
	public static class RaidPatterns
	{
		public const string Role = @"(?<roleNumber>[0-9]{1,3})\.(?<roleName>([a-zA-Zа-яА-Я0-9]|\s|\(|\)|/|-|'|\[|\]|{|}){2,})-(?<memberId>(<@[0-9]{1,}>|))?";
		public const string RoleNumber = @"[0-9]{1,3}";
		public const string RoleDelete = @"-[0-9]{1,3}";
	}
}
