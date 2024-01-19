﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;

namespace KorgiBot.Commands.Autocomplete
{
	public class AutocompleteProvider : IAutocompleteProvider
	{
		private object _cacheSync = new object();

		public List<DiscordAutoCompleteChoice> Choices { get; set; } = new List<DiscordAutoCompleteChoice>();

		public virtual async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext context)
		{
			return ReadChoices();
		}

		private List<DiscordAutoCompleteChoice> ReadChoices()
		{
			if (Choices.Any()) return Choices;

			lock (_cacheSync)
			{
				var configPath = Path.Combine(BotEnvironment.AutocompleteProvidersChoicesPath, $"{GetType().Name}.json");
				return Choices = JsonConvert.DeserializeObject<List<DiscordAutoCompleteChoice>>(File.ReadAllText(configPath));
			}
		}
	}
}
