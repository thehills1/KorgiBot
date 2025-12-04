using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using KorgiBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KorgiBot.Commands.Autocomplete
{
	public class RaidsAutocompleteProvider : AutocompleteProvider
	{
		public override async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext context)
		{
			var activeRaids = await context.Services
				.GetService<DatabaseContext>()
				.Raids
				.Where(raid => raid.GuildId == context.Guild.Id)
				.ToListAsync();

			return activeRaids.Select(raid => 
					new DiscordAutoCompleteChoice($"Thread: {raid.StartTime} Id: {raid.ThreadId}", raid.ThreadId.ToString()));
		}
	}
}