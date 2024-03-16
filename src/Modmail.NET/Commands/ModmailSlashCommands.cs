using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Commands;

public class ModmailSlashCommands : ApplicationCommandModule
{
  [SlashCommand("set-priority", "Set the priority of a ticket.")]
  [RequireUserPermissions(Permissions.Administrator)]
  public async Task SetPriority(InteractionContext ctx,
                                [Option("priority", "Priority of the ticket")]
                                TicketPriority priority) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());


    var currentChannel = ctx.Channel;
    var currentGuild = ctx.Guild;
    var currentMember = ctx.Member;
    var currentUser = ctx.User;
    var channelTopic = currentChannel.Topic;

    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
    if (ticketId == Guid.Empty) {
      var embed4 = ModmailEmbedBuilder.Base("This command can only be used in a ticket channel.","",DiscordColor.Green);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var ticket = await dbService.GetActiveTicketAsync(ticketId);
    if (ticket is null) {
      Log.Verbose("Active ticket not found: {TicketId} {GuildId}", ticketId, currentGuild.Id);
      
      var embed4 = ModmailEmbedBuilder.Base("This command can only be used in a ticket channel.","",DiscordColor.Green);

      
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var oldPriority = ticket.Priority;
    ticket.Priority = priority;
    await dbService.UpdateTicketAsync(ticket);


    // var guildId = ticket.GuildId;
    var ticketOpenUser = await currentGuild.GetMemberAsync(ticket.DiscordUserId);


    var newChName = "";
    switch (priority) {
      case TicketPriority.Normal:
        newChName = Const.NORMAL_PRIORITY_EMOJI + ctx.Channel.Name;
        break;
      case TicketPriority.High:
        newChName = Const.HIGH_PRIORITY_EMOJI + string.Format(Const.TICKET_NAME_TEMPLATE, ticketOpenUser.Username.Trim());
        ;
        break;
      case TicketPriority.Low:
        newChName = Const.LOW_PRIORITY_EMOJI + string.Format(Const.TICKET_NAME_TEMPLATE, ticketOpenUser.Username.Trim());
        ;
        break;
    }

    await ctx.Channel.ModifyAsync(x => { x.Name = newChName; });


    var embed = ModmailEmbedBuilder.ToUser.TicketPriorityChanged(ctx.Guild, ctx.User, oldPriority, priority);
    await ticketOpenUser.SendMessageAsync(embed);

    var embed2 = ModmailEmbedBuilder.ToLog.TicketPriorityChanged(ctx.Guild, ctx.User, oldPriority, priority);
    var logChannelId = await dbService.GetLogChannelIdAsync(ticket.GuildOption.GuildId);
    var logChannel = currentGuild.GetChannel(logChannelId);
    await logChannel.SendMessageAsync(embed2);


    var embed3 = ModmailEmbedBuilder.Base("Priority set!","",DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed3);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);

    Log.Information("Priority set: {TicketId} in guild {GuildId}", ticketId, ticket.GuildOption.GuildId);
  }

  [SlashCommand("toggle-sensitive-logging", "Toggle sensitive logging for the modmail bot.")]
  [RequireUserPermissions(Permissions.Administrator)]
  public async Task ToggleSensitiveLogging(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var ticketOption = await dbService.GetOptionAsync(currentGuildId);
    if (ticketOption is null) {
      var embed3 = ModmailEmbedBuilder.Base("Server not setup!","",DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    ticketOption.IsSensitiveLogging = !ticketOption.IsSensitiveLogging;
    await dbService.UpdateTicketOptionAsync(ticketOption);

    var text = new StringBuilder();
    if (ticketOption.IsSensitiveLogging) {
      text.Append("Sensitive logging enabled!");
      Log.Information("Sensitive logging enabled for guild: {GuildId}", currentGuildId);
    }
    else {
      text.Append("Sensitive logging disabled!");
      Log.Information("Sensitive logging disabled for guild: {GuildId}", currentGuildId);
    }

    var embed4 = ModmailEmbedBuilder.Base(text.ToString(),"",DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed4);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("setup", "Setup the modmail bot.")]
  [RequireUserPermissions(Permissions.Administrator)]
  public async Task Setup(InteractionContext ctx,
                          [Option("sensitive-logging", "Whether to log modmail messages")]
                          bool sensitiveLogging) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed3 = ModmailEmbedBuilder.Base("This command can only be used in the main server.","",DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var dbService = ServiceLocator.Get<IDbService>();

    // await using var db = new ModmailDbContext();
    var existingMmOption = await dbService.GetOptionAsync(currentGuildId);
    if (existingMmOption is not null) {
      var embed3 = ModmailEmbedBuilder.Base("Server already setup!","",DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var mainGuild = ctx.Guild;
    var category = await mainGuild.CreateChannelCategoryAsync(Const.CATEGORY_NAME);
    var logChannel = await mainGuild.CreateTextChannelAsync(Const.LOG_CHANNEL_NAME, category);
    var guildOption = new GuildOption {
      CategoryId = category.Id,
      GuildId = mainGuild.Id,
      LogChannelId = logChannel.Id,
      IsSensitiveLogging = sensitiveLogging,
      IsEnabled = true,
      RegisterDate = DateTime.Now,
      TakeFeedbackAfterClosing = false,
      ShowConfirmationWhenClosingTickets = false,
      AllowAnonymousResponding = false
    };
    await dbService.AddTicketOptionAsync(guildOption);

    var embed2 = ModmailEmbedBuilder.Base("Server setup complete!","",DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
    Log.Information("Server setup complete for guild: {GuildId}", currentGuildId);
  }


  [SlashCommand("close", "Close a ticket.")]
  [RequireUserPermissions(Permissions.Administrator)]
  public async Task CloseTicket(InteractionContext ctx,
                                [Option("reason", "Ticket closing reason")]
                                string reason = "") {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentChannel = ctx.Channel;
    var currentGuild = ctx.Guild;
    var currentMember = ctx.Member;
    var currentUser = ctx.User;
    var channelTopic = currentChannel.Topic;


    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
    if (ticketId == Guid.Empty) {
      var embed3 = ModmailEmbedBuilder.Base("This command can only be used in a ticket channel.","",DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var ticket = await dbService.GetActiveTicketAsync(ticketId);
    if (ticket is null) {
      Log.Verbose("Active ticket not found: {TicketId} {GuildId}", ticketId, currentGuild.Id);
      var embed3 = ModmailEmbedBuilder.Base("This command can only be used in a ticket channel.","",DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    ticket.ClosedDate = DateTime.Now;
    await dbService.UpdateTicketAsync(ticket);

    var ticketOpenUser = await currentGuild.GetMemberAsync(ticket.DiscordUserId);
    var embed = ModmailEmbedBuilder.ToUser.TicketClosed(ctx.Guild, ticketOpenUser);
    await ticketOpenUser.SendMessageAsync(embed);


    var logChannelId = await dbService.GetLogChannelIdAsync(ticket.GuildOption.GuildId);
    var logChannel = currentGuild.GetChannel(logChannelId);
    var logEmbed = ModmailEmbedBuilder.ToLog.TicketClosed(currentUser, ticketOpenUser, ticketId, ticket.RegisterDate, reason);
    await logChannel.SendMessageAsync(logEmbed);


    var embed2 = ModmailEmbedBuilder.Base("Ticket closed!","",DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);


    await currentChannel.DeleteAsync("ticket_closed");

    Log.Information("Ticket closed: {TicketId} in guild {GuildId}", ticketId, ticket.GuildOption.GuildId);
  }


  [SlashCommand("tag", "Bot sends defined message content by tag.")]
  [RequireUserPermissions(Permissions.Administrator)]
  public async Task Tag(InteractionContext ctx,
                        [ChoiceProvider(typeof(TagChoiceProvider))] [Option("key", "Tag key")]
                        string key) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var tag = await dbService.GetTagAsync(currentGuildId, key);
    if (tag is null) {
      var embed2 = ModmailEmbedBuilder.Base("Tag not found!","",DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var builder2 = new DiscordWebhookBuilder().WithContent(tag.MessageContent);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("add-tag", "Add a tag.")]
  [RequireUserPermissions(Permissions.Administrator)]
  public async Task AddTag(InteractionContext ctx,
                           [Option("key", "Tag key")] string key,
                           [Option("message", "Tag message")] string message) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var existingTag = await dbService.GetTagAsync(currentGuildId, key);
    if (existingTag is not null) {
      var embed2 = ModmailEmbedBuilder.Base("Tag already exists!","",DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var tag = new Tag {
      GuildId = currentGuildId,
      Key = key,
      MessageContent = message,
      RegisterDate = DateTime.Now,
      UseEmbed = false,
    };
    await dbService.AddTagAsync(tag);

    var embed = ModmailEmbedBuilder.Base("Tag added!","",DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
     
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("remove-tag", "Remove a tag.")]
  [RequireUserPermissions(Permissions.Administrator)]
  public async Task RemoveTag(InteractionContext ctx,
                              [ChoiceProvider(typeof(TagChoiceProvider))] [Option("key", "Tag key")]
                              string key) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var tag = await dbService.GetTagAsync(currentGuildId, key);
    if (tag is null) {
      var embed2 = ModmailEmbedBuilder.Base("Tag not found!","",DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    await dbService.RemoveTagAsync(tag);

    var embed = ModmailEmbedBuilder.Base("Tag removed!","",DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }
  
  [SlashCommand("list-tag", "List all tags.")]
  [RequireUserPermissions(Permissions.Administrator)]
  public async Task ListTag(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var tags = await dbService.GetTagsAsync(currentGuildId);
    if (tags is null || tags.Count == 0) {
      var embed2 = ModmailEmbedBuilder.Base("No tags found!","",DiscordColor.Red);

      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var embed = ModmailEmbedBuilder.ListTags(ctx.Guild, tags);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }
  
  

}