namespace KorgiBot.Server.Raids.Commands
{
	public class CommandParseResult
	{
		public bool Result { get; }

		public CommandContext Context { get; }

		public CommandParseResult(bool result, CommandContext context)
		{
			Result = result;
			Context = context;
		}
	}
}
