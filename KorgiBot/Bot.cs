using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.AsyncEvents;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using KorgiBot.Commands;
using KorgiBot.Extensions;

namespace KorgiBot
{
	public class Bot : IDisposable
    {
		private bool _isInitialized = false;

        private readonly DiscordClient _client;
        private readonly IServiceProvider _serviceProvider;

        public Bot(DiscordClient client, IServiceProvider serviceProvider)
        {
            _client = client;
            _serviceProvider = serviceProvider;
        }

        public async void Initialize()
        {
			if (_isInitialized) return;

			SetupInteractivity();
			SetupCommandsRegistration();
			await RunAsync();

			_isInitialized = true;
        }

		#region Messages
		public async Task<bool> MessageExistsAsync(ulong channelId, ulong messageId) => await GetMessageAsync(channelId, messageId) != null;

		public async Task<bool> MessageExistsAsync(DiscordChannel channel, ulong messageId) => await GetMessageAsync(channel, messageId) != null;

		public async Task<DiscordMessage> GetMessageAsync(ulong channelId, ulong messageId)
		{
			var channel = await GetChannelAsync(channelId);
			return await GetMessageAsync(channel, messageId);
		}

		public async Task<DiscordMessage> GetMessageAsync(DiscordChannel channel, ulong messageId)
		{
			if (channel == null) return null;

			try
			{
				return await channel.GetMessageAsync(messageId);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while getting message, [messageId]=[{messageId}]:\n{e}");
				return null;
			}
		}

		public async Task<DiscordMessage> SendMessageAsync(ulong channelId, string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			var channel = await GetChannelAsync(channelId);
			return await SendMessageAsync(channel, content, embeds, file);
		}

		public async Task<DiscordMessage> SendMessageAsync(DiscordChannel channel, string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			if (channel == null) return null;

			var messageBuilder = GenerateMessage(content, embeds, file);

			try
			{
				return await messageBuilder.SendAsync(channel);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while sending message, [channelId]=[{channel?.Id}]\n{e}");
				return null;
			}
		}

		public async Task<DiscordMessage> EditMessageAsync(ulong channelId, ulong messageId, string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			var message = await GetMessageAsync(channelId, messageId);
			if (message == null) return null;

			return await EditMessageAsync(message, content, embeds, file);
		}

		public async Task<DiscordMessage> EditMessageAsync(DiscordChannel channel, ulong messageId, string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			var message = await GetMessageAsync(channel, messageId);
			if (message == null) return null;

			return await EditMessageAsync(message, content, embeds, file);
		}

		public async Task<DiscordMessage> EditMessageAsync(DiscordMessage message, string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			var messageBuilder = GenerateMessage(content, embeds, file);

			try
			{
				return await message.ModifyAsync(messageBuilder);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while editing message, [messageId]=[{message?.Id}]\n{e}");
				return null;
			}
		}

		public async Task DeleteMessageAsync(DiscordChannel channel, ulong messageId)
		{
			var message = await GetMessageAsync(channel, messageId);
			await DeleteMessageAsync(message);
		}

		public async Task DeleteMessageAsync(ulong channelId, ulong messageId)
		{
			var message = await GetMessageAsync(channelId, messageId);
			await DeleteMessageAsync(message);
		}

		public async Task DeleteMessageAsync(DiscordMessage message)
		{
			if (message == null) return;

			try
			{
				await message.DeleteAsync();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while deleting message, [messageId]=[{message.Id}]:\n{e}");
				return;
			}
		}

		private DiscordMessageBuilder GenerateMessage(string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			var messageBuilder = new DiscordMessageBuilder();

			messageBuilder = messageBuilder.WithAllowedMentions(Mentions.All);

			if (!content.IsNullOrEmpty()) messageBuilder = messageBuilder.WithContent(content);
			if (embeds != null || (embeds?.Any() ?? false)) messageBuilder = messageBuilder.AddEmbeds(embeds);
			if (file != null) messageBuilder = messageBuilder.AddFile(file.Name, file);

			return messageBuilder;
		}
		#endregion

		#region Channels
		public async Task<DiscordChannel> GetChannelAsync(ulong channelId)
		{
			try
			{
				return await _client.GetChannelAsync(channelId);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while getting channel, [channelId]=[{channelId}]:\n{e}");
				return null;
			}
		}

		public async Task DeleteChannelAsync(ulong channelId)
		{
			var channel = await GetChannelAsync(channelId);
			if (channel == null) return;

			await channel.DeleteAsync();
		}

		public async Task<List<DiscordChannel>> GetChannelsAsync(ulong guildId, Func<DiscordChannel, bool> selector)
		{
			var channels = await GetChannelsAsync(guildId);
			return channels.Where(selector).ToList();
		}

		public async Task<IReadOnlyCollection<DiscordChannel>> GetChannelsAsync(ulong guildId)
		{
			var guild = await GetGuildAsync(guildId);

			try
			{
				return await guild.GetChannelsAsync();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while getting all guild's channels, [guildId]=[{guildId}]\n{e}");
				return null;
			}
		}
		#endregion

		#region Guilds
		public async Task<DiscordGuild> GetGuildAsync(ulong guildId)
		{
			try
			{
				return await _client.GetGuildAsync(guildId);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while getting guild, [guildId]=[{guildId}]:\n{e}");
				return null;
			}
		}
		#endregion

		#region Users
		public async Task<DiscordUser> GetUserAsync(ulong userId)
		{
			try
			{
				return await _client.GetUserAsync(userId);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while getting user, [userId]=[{userId}]\n{e}");
				return null;
			}
		}

		public async Task<DiscordMember> GetMemberAsync(ulong guildId, ulong memberId)
		{
			var guild = await GetGuildAsync(guildId);

			try
			{
				return await guild.GetMemberAsync(memberId);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while getting guild's member, [guildId]=[{guildId}], [memberId]=[{memberId}]\n{e}");
				return null;
			}
		}

		public async Task<DiscordMessage> SendDirectMessage(ulong guildId, ulong memberId, string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			var member = await GetMemberAsync(guildId, memberId);
			return await SendDirectMessage(member, content, embeds, file);
		}

		public async Task<DiscordMessage> SendDirectMessage(DiscordMember member, string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			if (member == null) return null;

			var dmChannel = await member.CreateDmChannelAsync();
			var messageBuilder = GenerateMessage(content, embeds, file);

			return await dmChannel.SendMessageAsync(messageBuilder);
		}
		#endregion

		#region Reactions
		public async Task SetReactionAsync(DiscordMessage message, string reactionName)
		{
			await SetReactionAsync(message, GetReactionByName(reactionName));
		}

		public async Task SetReactionAsync(DiscordMessage message, DiscordEmoji reaction)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			if (reaction == null)
			{
				throw new ArgumentNullException(nameof(reaction));
			}

			await message.CreateReactionAsync(reaction);
		}

		public DiscordEmoji GetReactionByName(string name)
		{
			if (name.IsNullOrEmpty()) return null;

			return DiscordEmoji.FromName(_client, name);
		}
		#endregion

		#region Threads
		public async Task DeleteThreadAsync(ulong threadId)
		{
			try
			{
				var thread = await _client.GetChannelAsync(threadId);
				await thread.DeleteAsync();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while deleting thread, [threadId]=[{threadId}]:\n{e}");
			}
		}
		#endregion

		public event AsyncEventHandler<DiscordClient, MessageCreateEventArgs> MessageCreated
		{
			add { _client.MessageCreated += value; }
			remove { _client.MessageCreated -= value; }
		}

		private void SetupInteractivity()
		{
			_client.UseInteractivity(new InteractivityConfiguration()
			{
				Timeout = TimeSpan.FromMinutes(5)
			});

			_client.ModalSubmitted += async (s, e) => await e.Interaction.DeferAsync(true);
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
