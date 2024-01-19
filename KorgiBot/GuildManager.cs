using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace KorgiBot
{
	public class GuildManager
	{
		private readonly Bot _bot;

		public GuildManager(Bot bot)
		{
			_bot = bot;
		}

		public async Task<DiscordMessage> SendMessageAsync(ulong channelId, string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			return await SendMessageAsync(await _bot.GetChannelAsync(channelId), content, embeds, file);
		}

		public async Task<DiscordMessage> SendMessageAsync(DiscordChannel channel, string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			var messageBuilder = new DiscordMessageBuilder();

			if (content != null) messageBuilder = messageBuilder.WithContent(content);
			if (file != null) messageBuilder = messageBuilder.AddFile(file);

			if (embeds != null)
			{
				foreach (var embed in embeds)
				{
					messageBuilder = messageBuilder.AddEmbed(embed);
				}
			}

			messageBuilder = messageBuilder.WithAllowedMentions(Mentions.All);

			return await messageBuilder.SendAsync(channel);
		}

		public async Task<List<DiscordMessage>> SendMessagesAsync(DiscordChannel channel, List<string> contents)
		{
			var output = new List<DiscordMessage>();

			foreach (var content in contents)
			{
				output.Add(await SendMessageAsync(channel, content));
			}

			return output;
		}

		public async Task SetReactionAsync(DiscordMessage message, string reactionName)
		{
			await SetReaction(message, _bot.GetReactionByName(reactionName));
		}

		public async Task SetReaction(DiscordMessage message, DiscordEmoji reaction)
		{
			await message.CreateReactionAsync(reaction);
		}

		public async Task<List<DiscordMessage>> EditMessagesAsync(ulong channelId, List<ulong> messageIds, List<string> contents)
		{
			if (messageIds.Count != contents.Count) return null;

			var channel = await GetChannelAsync(channelId);
			var messages = new List<DiscordMessage>();
			for (int i = 0; i < messageIds.Count; i++)
			{
				messages.Add(await channel.GetMessageAsync(messageIds[i]));
			}

			return await EditMessagesAsync(messages, contents);
		}

		public async Task<List<DiscordMessage>> EditMessagesAsync(List<DiscordMessage> messages, List<string> contents)
		{
			if (messages.Count != contents.Count) return null;

			var output = new List<DiscordMessage>();
			for (int i = 0; i < messages.Count; i++)
			{
				output.Add(await messages[i].ModifyAsync(contents[i]));
			}

			return output;
		}

		public async Task<DiscordMessage> EditMessageAsync(ulong channelId, ulong messageId, string content = null, List<DiscordEmbed> embeds = null)
		{
			return await EditMessageAsync(await GetChannelAsync(channelId), messageId, content, embeds);
		}

		public async Task<DiscordMessage> EditMessageAsync(DiscordChannel channel, ulong messageId, string content = null, List<DiscordEmbed> embeds = null)
		{
			return await EditMessageAsync(await channel.GetMessageAsync(messageId), content, embeds);
		}

		public async Task<DiscordMessage> EditMessageAsync(DiscordMessage message, string content = null, List<DiscordEmbed> embeds = null)
		{
			return await message.ModifyAsync(content, embeds);
		}

		public async Task DeleteMessageAsync(DiscordMessage message)
		{
			await message.DeleteAsync();
		}

		public async Task DeleteMessageAsync(ulong channelId, ulong messageId)
		{
			await (await _bot.GetMessageAsync(channelId, messageId)).DeleteAsync();
		}

		public async Task<DiscordMessage> GetMessageAsync(ulong channelId, ulong messageId) => await _bot.GetMessageAsync(channelId, messageId);

		public async Task<DiscordChannel> GetChannelAsync(ulong channelId) => await _bot.GetChannelAsync(channelId);

		public async Task<DiscordUser> GetUserAsync(ulong userId) => await _bot.GetUserAsync(userId);

		public async Task DeleteChannelAsync(ulong channelId) => await _bot.DeleteChannelAsync(channelId);

		public async Task<DiscordMember> GetUserInGuild(ulong guildId, ulong userId) => await _bot.GetUserInGuild(guildId, userId);

		public async Task<IReadOnlyList<DiscordChannel>> GetAllChannels(ulong guildId) => await _bot.GetAllChannels(guildId);
	}
}
