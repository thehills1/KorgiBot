using System.Threading.Tasks;
using Default.Langs;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using KorgiBot.Configs;
using KorgiBot.Extensions;
using KorgiBot.Langs;

namespace KorgiBot.Server.Commands
{
	public abstract class BaseServerCommands
	{
		protected async Task SendCommandExecutionResult(InteractionContext context, ServerConfig serverConfig, CommandResult result)
		{
			if (context == null || serverConfig == null) return;
			if (result == null)
			{
				await context.EditResponseAsync(new DiscordWebhookBuilder()
					.AddEmbedWithErrorResult(
					TranslationKeys.CommandExecutionError.Translate(serverConfig.ServerLanguage), 
					TranslationKeys.CommandExecutionUnexpectedError.Translate(serverConfig.ServerLanguage)));

				return;
			} 

			if (result.Success)
			{
				await context.EditResponseAsync(new DiscordWebhookBuilder()
					.AddEmbedWithSuccessResult(TranslationKeys.CommandExecutedSuccessfully.Translate(serverConfig.ServerLanguage), result.Message));
			}
			else
			{
				await context.EditResponseAsync(new DiscordWebhookBuilder()
					.AddEmbedWithErrorResult(TranslationKeys.CommandExecutionError.Translate(serverConfig.ServerLanguage), result.Message));
			}
		}

		protected async Task SendCommandExecutionResult(DiscordInteraction interaction, ServerConfig serverConfig, CommandResult result)
		{
			if (interaction == null || serverConfig == null) return;
			if (result == null)
			{
				await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
					.AddEmbedWithErrorResult(
					TranslationKeys.CommandExecutionError.Translate(serverConfig.ServerLanguage),
					TranslationKeys.CommandExecutionUnexpectedError.Translate(serverConfig.ServerLanguage)));

				return;
			} 

			if (result.Success)
			{
				await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
					.AddEmbedWithSuccessResult(TranslationKeys.CommandExecutedSuccessfully.Translate(serverConfig.ServerLanguage), result.Message));
			}
			else
			{
				await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
					.AddEmbedWithErrorResult(TranslationKeys.CommandExecutionError.Translate(serverConfig.ServerLanguage), result.Message));
			}
		}
	}
}
