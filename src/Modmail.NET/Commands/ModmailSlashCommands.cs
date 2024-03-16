using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Commands;

public class ModmailSlashCommands : ApplicationCommandModule
{
  [SlashCommand("set-priority", "Set the priority of a ticket. This command changes the channel name. It can only be used in a ticket channel.")]
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
      var builder = new DiscordWebhookBuilder().WithContent("This command can only be used in a ticket channel.");
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var ticket = await dbService.GetActiveTicketAsync(ticketId);
    if (ticket is null) {
      Log.Verbose("Active ticket not found: {TicketId} {GuildId}", ticketId, currentGuild.Id);
      var builder = new DiscordWebhookBuilder().WithContent("This command can only be used in a ticket channel.");
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }



    var oldPriority = ticket.Priority;
    ticket.Priority = priority;
    await dbService.UpdateTicketAsync(ticket);

    var guildId = ticket.GuildId;
    var ticketOpenUser = await currentGuild.GetMemberAsync(ticket.DiscordUserId);
    
    
    var newChName = "";
    switch (priority) {
      case TicketPriority.Normal:
        newChName = Const.NORMAL_PRIORITY_EMOJI + ctx.Channel.Name;
        break;
      case TicketPriority.High:
        newChName = Const.HIGH_PRIORITY_EMOJI + string.Format(Const.TICKET_NAME_TEMPLATE, ticketOpenUser.Username.Trim()); ;
        break;
      case TicketPriority.Low :
        newChName = Const.LOW_PRIORITY_EMOJI + string.Format(Const.TICKET_NAME_TEMPLATE, ticketOpenUser.Username.Trim()); ;
        break;
    }
    await ctx.Channel.ModifyAsync(x => {
      x.Name =  newChName;
    });
    
    
    
    
    var embed = ModmailEmbedBuilder.ToUser.TicketPriorityChanged(ctx.Guild, ctx.User,oldPriority, priority);
    await ticketOpenUser.SendMessageAsync(embed);

    var embed2 = ModmailEmbedBuilder.ToLog.TicketPriorityChanged(ctx.Guild,ctx.User,oldPriority,priority);
    var logChannelId = await dbService.GetLogChannelIdAsync(guildId);
    var logChannel = currentGuild.GetChannel(logChannelId);
    await logChannel.SendMessageAsync(embed2);


    
    var builder2 = new DiscordWebhookBuilder().WithContent("Priority set!");
    await ctx.Interaction.EditOriginalResponseAsync(builder2);

    Log.Information("Priority set: {TicketId} in guild {GuildId}", ticketId, guildId);
  }

  [SlashCommand("toggle-sensitive-logging", "Toggle sensitive logging for the modmail bot.")]
  [RequireUserPermissions(Permissions.Administrator)]
  public async Task ToggleSensitiveLogging(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var ticketOption = await dbService.GetOptionAsync(currentGuildId);
    if (ticketOption is null) {
      var builder = new DiscordWebhookBuilder().WithContent("Server not setup!");
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

    var builder2 = new DiscordWebhookBuilder().WithContent(text.ToString());
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
      var builder = new DiscordWebhookBuilder().WithContent("This command can only be used in the main server.");
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var dbService = ServiceLocator.Get<IDbService>();

    // await using var db = new ModmailDbContext();
    var existingMmOption = await dbService.GetOptionAsync(currentGuildId);
    if (existingMmOption is not null) {
      var builder = new DiscordWebhookBuilder().WithContent("Server already setup!");
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var mainGuild = ctx.Guild;
    var category = await mainGuild.CreateChannelCategoryAsync(Const.CATEGORY_NAME);
    var logChannel = await mainGuild.CreateTextChannelAsync(Const.LOG_CHANNEL_NAME, category);
    var mmOption = new TicketOption {
      CategoryId = category.Id,
      GuildId = mainGuild.Id,
      LogChannelId = logChannel.Id,
      IsSensitiveLogging = sensitiveLogging,
      IsEnabled = true,
      RegisterDate = DateTime.Now,
    };
    await dbService.AddTicketOptionAsync(mmOption);


    var builder2 = new DiscordWebhookBuilder().WithContent("Server setup complete!");
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
      var builder = new DiscordWebhookBuilder().WithContent("This command can only be used in a ticket channel.");
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var ticket = await dbService.GetActiveTicketAsync(ticketId);
    if (ticket is null) {
      Log.Verbose("Active ticket not found: {TicketId} {GuildId}", ticketId, currentGuild.Id);
      var builder = new DiscordWebhookBuilder().WithContent("This command can only be used in a ticket channel.");
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var guildId = ticket.GuildId;

    ticket.ClosedDate = DateTime.Now;
    await dbService.UpdateTicketAsync(ticket);

    var ticketOpenUser = await currentGuild.GetMemberAsync(ticket.DiscordUserId);
    var embed = ModmailEmbedBuilder.ToUser.TicketClosed(ctx.Guild, ticketOpenUser);
    await ticketOpenUser.SendMessageAsync(embed);


    var logChannelId = await dbService.GetLogChannelIdAsync(guildId);
    var logChannel = currentGuild.GetChannel(logChannelId);
    var logEmbed = ModmailEmbedBuilder.ToLog.TicketClosed(currentUser, ticketOpenUser, ticketId, ticket.RegisterDate, reason);
    await logChannel.SendMessageAsync(logEmbed);


    var builder2 = new DiscordWebhookBuilder().WithContent("Ticket closed!");
    await ctx.Interaction.EditOriginalResponseAsync(builder2);


    await currentChannel.DeleteAsync("ticket_closed");

    Log.Information("Ticket closed: {TicketId} in guild {GuildId}", ticketId, guildId);
  }
}