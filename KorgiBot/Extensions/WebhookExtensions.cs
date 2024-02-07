using DSharpPlus.Entities;

namespace KorgiBot.Extensions
{
    public static class WebhookExtensions
    {
        public static DiscordEmbedBuilder WithSuccessResult(this DiscordEmbedBuilder builder, string title, string description)
        {
            return builder.WithColor(DiscordRgbColor.Green).WithTitle(title).WithDescription(description);
        }

        public static DiscordEmbedBuilder WithErrorResult(this DiscordEmbedBuilder builder, string title, string description)
        {
            return builder.WithColor(DiscordRgbColor.Red).WithTitle(title).WithDescription(description);
        }

        public static DiscordWebhookBuilder AddEmbedWithSuccessResult(this DiscordWebhookBuilder builder, string title, string description)
        {
            return builder.AddEmbed(new DiscordEmbedBuilder().WithSuccessResult(title, description));
        }

        public static DiscordWebhookBuilder AddEmbedWithErrorResult(this DiscordWebhookBuilder builder, string title, string description)
        {
            return builder.AddEmbed(new DiscordEmbedBuilder().WithErrorResult(title, description));
        }

        public static DiscordInteractionResponseBuilder AddEmbedWithSuccessResult(this DiscordInteractionResponseBuilder builder, string title, string content)
        {
            return builder.AddEmbed(new DiscordEmbedBuilder().WithSuccessResult(title, content));
        }

        public static DiscordInteractionResponseBuilder AddEmbedWithErrorResult(this DiscordInteractionResponseBuilder builder, string title, string content)
        {
            return builder.AddEmbed(new DiscordEmbedBuilder().WithErrorResult(title, content));
        }
    }
}
