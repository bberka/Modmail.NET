using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Providers;
using Modmail.NET.Static;

namespace Modmail.NET.Commands;

[SlashCommandGroup("tag", "Tag management commands.")]
public class TagSlashCommands : ApplicationCommandModule
{
  [SlashCommand("list", "List all tags.")]
  public async Task ListTag(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var tags = await dbService.GetTagsAsync(currentGuildId);
    if (tags is null || tags.Count == 0) {
      var embed2 = ModmailEmbedBuilder.Base("No tags found!", "", DiscordColor.Red);

      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var embed = ModmailEmbedBuilder.ListTags(ctx.Guild, tags);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("get", "Bot sends defined message content by tag.")]
  public async Task Tag(InteractionContext ctx,
                        [Autocomplete(typeof(TagProvider))] [Option("key", "Tag key")]
                        string key) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var tag = await dbService.GetTagAsync(currentGuildId, key);
    if (tag is null) {
      var embed2 = ModmailEmbedBuilder.Base("Tag not found!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var builder2 = new DiscordWebhookBuilder();

    if (tag.UseEmbed) {
      var embed = ModmailEmbedBuilder.Base(tag.Key, tag.MessageContent, DiscordColor.Green);
      builder2.AddEmbed(embed);
    }
    else {
      builder2.WithContent(tag.MessageContent);
    }

    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("add", "Add a tag.")]
  [RequirePermissionLevelOrHigher(TeamPermissionLevel.Moderator)]
  public async Task AddTag(InteractionContext ctx,
                           [Option("key", "Tag key")] string key,
                           [Option("message", "Tag message")] string message,
                           [Option("use-embed", "Whether to use embed")]
                           bool useEmbed = false) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var existingTag = await dbService.GetTagAsync(currentGuildId, key);
    if (existingTag is not null) {
      var embed2 = ModmailEmbedBuilder.Base("Tag already exists!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var tag = new Tag {
      GuildOptionId = currentGuildId,
      Key = key,
      MessageContent = message,
      RegisterDate = DateTime.Now,
      UseEmbed = useEmbed
    };
    await dbService.AddTagAsync(tag);

    var embed = ModmailEmbedBuilder.Base("Tag added!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);

    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("remove", "Remove a tag.")]
  [RequirePermissionLevelOrHigher(TeamPermissionLevel.Moderator)]
  public async Task RemoveTag(InteractionContext ctx,
                              [Autocomplete(typeof(TagProvider))] [Option("key", "Tag key")]
                              string key) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var tag = await dbService.GetTagAsync(currentGuildId, key);
    if (tag is null) {
      var embed2 = ModmailEmbedBuilder.Base("Tag not found!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    await dbService.RemoveTagAsync(tag);

    var embed = ModmailEmbedBuilder.Base("Tag removed!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }
}