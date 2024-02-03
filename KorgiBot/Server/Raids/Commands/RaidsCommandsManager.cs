using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using KorgiBot.Configs;

namespace KorgiBot.Server.Raids.Commands
{
	public class RaidsCommandsManager
	{
		private readonly Bot _bot;
		private readonly RaidsManager _raidsManager;
		private readonly RaidsConfig _raidsConfig;

		private readonly List<ICommand> _commands = new();

		public RaidsCommandsManager(Bot bot, RaidsManager raidsManager, RaidsConfig raidsConfig)
		{
			_bot = bot;
			_raidsManager = raidsManager;
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

		public async Task HandleCommand(MessageCreateEventArgs args)
		{
			foreach (var command in _commands)
			{
				var parseResult = await command.TryParse(args);
				if (parseResult.Result)
				{
					var result = await command.TryExecute(parseResult.Context);
					await SendCommandExecutionResult(args.Message, result);
					break;
				}
			}
		}

		private async Task SendCommandExecutionResult(DiscordMessage message, bool result)
		{
			if (result)
			{
				await _bot.SetReactionAsync(message, _raidsConfig.PositiveReactionName);
			}
			else
			{
				await _bot.SetReactionAsync(message, _raidsConfig.NegativeReactionName);
			}
		}
	}
}
