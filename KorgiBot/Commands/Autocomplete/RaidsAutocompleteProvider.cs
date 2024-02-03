using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;

namespace KorgiBot.Commands.Autocomplete
{
	public class RaidsAutocompleteProvider : AutocompleteProvider
	{
		public override async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext context)
		{
			try
			{
				return context.Services.GetService<ServiceManager>().GetServerService(context.Guild.Id).RaidsManager.ActiveRaids.Values.Select(value => value.Info)
					.Select(raidInfo => new DiscordAutoCompleteChoice($"Thread: {raidInfo.Thread.Name} Id: {raidInfo.Thread.Id}", raidInfo.Thread.Id.ToString()));
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return null;
		}
	}
}
