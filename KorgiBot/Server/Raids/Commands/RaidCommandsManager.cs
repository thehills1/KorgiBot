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
	public class RaidCommandsManager
	{
		private static readonly List<ConstructorInfo> CommandConstructors = [];

		private readonly List<ICommand> _commands = new();

		private readonly Bot _bot;
		private readonly RaidsManager _raidsManager;
		private readonly RaidsConfig _raidsConfig;

		static RaidCommandsManager()
		{
			foreach (var type in Assembly.GetAssembly(typeof(ICommand)).GetTypes())
			{
				if (!type.IsAssignableTo(typeof(ICommand)) || type == typeof(ICommand))
				{
					continue;
				}

				CommandConstructors.Add(type.GetConstructors().First());
			}
		}

		public RaidCommandsManager(Bot bot, RaidsManager raidsManager, RaidsConfig raidsConfig)
		{
			_bot = bot;
			_raidsManager = raidsManager;
			_raidsConfig = raidsConfig;
		}

		public void Initialize()
		{
			foreach (var constructor in CommandConstructors)
			{
				_commands.Add(constructor.Invoke([_bot, _raidsManager]) as ICommand);
			}

			Console.WriteLine($"Commands {_commands.Count}");
		}

		public async Task HandleCommandAsync(MessageCreateEventArgs args)
		{
			foreach (var command in _commands)
			{
				var parseResult = await command.TryParse(args);
				if (!parseResult.Result) continue;

				var result = await command.TryExecute(parseResult.Context);
				await SendCommandExecutionResult(args.Message, result);

				break;
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