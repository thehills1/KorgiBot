using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using KorgiBot.Configs;

namespace KorgiBot.Server.Raids.Commands
{
	public class RaidsCommandsManager
	{
		private readonly Bot _bot;
		private readonly RaidsManager _raidsManager;
		private readonly GuildManager _guildManager;
		private readonly RaidsConfig _raidsConfig;

		private readonly List<ICommand> _commands = new();

		public RaidsCommandsManager(Bot bot, RaidsManager raidsManager, GuildManager guildManager, RaidsConfig raidsConfig)
		{
			_bot = bot;
			_raidsManager = raidsManager;
			_guildManager = guildManager;
			_raidsConfig = raidsConfig;
		}

		public void Initialize()
		{
			foreach (var type in Assembly.GetAssembly(typeof(ICommand)).GetTypes())
			{
				if (!type.IsAssignableTo(typeof(ICommand))) continue;
				if (type == typeof(ICommand)) continue;

				_commands.Add(type.GetConstructors().First().Invoke(new object[] { _bot, _raidsManager }) as ICommand);
			}

			Console.WriteLine($"Commands {_commands.Count}");
		}

		public void HandleCommand(MessageCreateEventArgs args)
		{
			foreach (var command in _commands)
			{
				if (command.TryParse(args, out var context))
				{
					var result = command.TryExecute(context);
					SendCommandExecutionResult(args.Message, result);
					break;
				}
			}
		}

		private void SendCommandExecutionResult(DiscordMessage message, bool result)
		{
			if (result)
			{
				_guildManager.SetReactionAsync(message, _raidsConfig.PositiveReactionName).Wait();
			}
			else
			{
				_guildManager.SetReactionAsync(message, _raidsConfig.NegativeReactionName).Wait();
			}
		}
	}
}
