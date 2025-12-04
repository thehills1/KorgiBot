namespace KorgiBot
{
	public static class DefaultSyncs
	{
		public static string UpdateRoles(long raidId) => $"raid:{raidId}:update_roles:all";

		public static string UpdateRole(long raidId, long roleId, long? transferFromRoleId = null) => $"raid:{raidId}:update_roles:{roleId}:transferFromRole:{transferFromRoleId}";
	}
}