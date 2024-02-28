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
			return context.Services
				.GetService<ServiceManager>()
				.GetServerService(context.Guild.Id).RaidsManager.ActiveRaids.Values
				.Select(raidProvider => 
					new DiscordAutoCompleteChoice($"Thread: {raidProvider.Raid.StartTime} Id: {raidProvider.Info.ThreadId}", raidProvider.Info.ThreadId.ToString()));
		}
	}
}
