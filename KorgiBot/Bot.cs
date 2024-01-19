using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.AsyncEvents;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using KorgiBot.Commands;

namespace KorgiBot
{
	public class Bot : IDisposable
    {
        private readonly DiscordClient _client;
        private readonly IServiceProvider _serviceProvider;

        public Bot(DiscordClient client, IServiceProvider serviceProvider)
        {
            _client = client;
            _serviceProvider = serviceProvider;
        }

        public async void Initialize()
        {
            SetupCommandsRegistration();
			await RunAsync();
        }

        public async Task<DiscordMessage> GetMessageAsync(ulong channelId, ulong messageId) => await (await GetChannelAsync(channelId)).GetMessageAsync(messageId);

        public async Task<DiscordChannel> GetChannelAsync(ulong id) => await _client.GetChannelAsync(id);

        public async Task<DiscordUser> GetUserAsync(ulong id) => await _client.GetUserAsync(id);

		public async Task DeleteChannelAsync(ulong channelId) => await (await GetChannelAsync(channelId)).DeleteAsync();

		public async Task<IReadOnlyList<DiscordChannel>> GetAllChannels(ulong guildId) => await (await _client.GetGuildAsync(guildId)).GetChannelsAsync();

		public async Task<DiscordMember> GetUserInGuild(ulong guildId, ulong userId) => await (await _client.GetGuildAsync(guildId)).GetMemberAsync(userId);

		public event AsyncEventHandler<DiscordClient, MessageCreateEventArgs> MessageCreated
		{
			add { _client.MessageCreated += value; }
			remove { _client.MessageCreated -= value; }
		}

		public DiscordEmoji GetReactionByName(string name)
		{
			return DiscordEmoji.FromName(_client, name);
		}

        private void SetupCommandsRegistration()
        {
            var cmds = _client.UseSlashCommands(new SlashCommandsConfiguration() { Services = _serviceProvider });

            _client.GuildAvailable += async (s, e) => await RegisterCommandsAndUpdate(cmds, e.Guild.Id);
            _client.GuildCreated += async (s, e) => await RegisterCommandsAndUpdate(cmds, e.Guild.Id);
        }

        private async Task RegisterCommandsAndUpdate(SlashCommandsExtension cmds, ulong guildId)
        {
            cmds.RegisterCommands<GlobalCommands>(guildId);
            await cmds.RefreshCommands();
        }

        private async Task RunAsync()
        {
            await _client.ConnectAsync();
            await Task.Delay(-1);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
