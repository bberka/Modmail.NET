using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Events;

public static class OnMessageCreated
{
  public static async Task Handle(DiscordClient sender, MessageCreateEventArgs args) {
    await Task.WhenAll(HandlePrivateMessage(sender, args.Message, args.Channel, args.Author),
                       HandleGuildMessage(sender, args.Message, args.Channel, args.Author, args.Guild));
  }

  internal static async Task HandlePrivateMessage(DiscordClient sender,
                                                  DiscordMessage message,
                                                  DiscordChannel channel,
                                                  DiscordUser author) {
    if (message.Author.IsBot) return;
    if (message.IsTTS) return;
    if (!channel.IsPrivate) return;
    var channelId = channel.Id;
    var authorId = author.Id;
    var guildId = MMConfig.This.MainServerId;

    if (message.Content.StartsWith(MMConfig.This.BotPrefix))
      //ignored
      return;

    var dbService = ServiceLocator.Get<IDbService>();
    //Check if user has active modmail
    // await using var db = new ModmailDbContext();
    var option = await dbService.GetOptionAsync(MMConfig.This.MainServerId);
    if (option is null) {
      Log.Error("Option not found for guild: {GuildOptionId}", guildId);
      return;
    }

    var activeTicket = await dbService.GetActiveTicketAsync(authorId);
    var guild = await sender.GetGuildAsync(option.GuildId);
    var logChannel = guild.GetChannel(option.LogChannelId);


    if (activeTicket is null) {
      //make new channel
      var channelName = string.Format(Const.TICKET_NAME_TEMPLATE, author.Username.Trim());
      var category = guild.GetChannel(option.CategoryId);

      var ticketId = Guid.NewGuid();

      var permissions = await dbService.GetPermissionInfoAsync(guildId);
      var members = await guild.GetAllMembersAsync();
      var roles = guild.Roles;

      var roleListForOverwrites = new List<DiscordRole>();
      var memberListForOverwrites = new List<DiscordMember>();
      foreach (var perm in permissions) {
        var role = roles.FirstOrDefault(x => x.Key == perm.Key && perm.Type == TeamMemberDataType.RoleId);
        if (role.Key != 0) roleListForOverwrites.Add(role.Value);
        var member2 = members.FirstOrDefault(x => x.Id == perm.Key && perm.Type == TeamMemberDataType.UserId);
        if (member2 is not null && member2.Id != 0) memberListForOverwrites.Add(member2);
      }


      var permissionOverwrites = UtilPermission.GetTicketPermissionOverwrites(guild, memberListForOverwrites, roleListForOverwrites);
      var mailChannel = await guild.CreateTextChannelAsync(channelName, category, UtilChannelTopic.BuildChannelTopic(ticketId), permissionOverwrites);

      var member = await guild.GetMemberAsync(author.Id);
      var embedNewTicket = ModmailEmbedBuilder.ToMail.NewTicket(member);
      var sb = new StringBuilder();
      if (roleListForOverwrites.Count > 0) {
        sb.AppendLine("Roles:");
        foreach (var role in roleListForOverwrites) sb.AppendLine(role.Mention);
      }

      if (memberListForOverwrites.Count > 0) {
        sb.AppendLine("Members:");
        foreach (var member2 in memberListForOverwrites) sb.AppendLine(member2.Mention);
      }


      await mailChannel.SendMessageAsync(sb.ToString(), embedNewTicket);

      var embedUserMessage = ModmailEmbedBuilder.ToMail.MessageReceived(author, message);
      await mailChannel.SendMessageAsync(embedUserMessage);


      var ticket = new Ticket {
        DiscordUserId = authorId,
        ModMessageChannelId = mailChannel.Id,
        RegisterDate = DateTime.Now,
        PrivateMessageChannelId = channelId,
        InitialMessageId = message.Id,
        Priority = TicketPriority.Normal,
        LastMessageDate = DateTime.Now,
        GuildOptionId = guildId,
        Id = ticketId,
        Anonymous = false,
        IsForcedClosed = false
      };

      await dbService.AddTicketAsync(ticket);


      var embedTicketCreated = ModmailEmbedBuilder.ToUser.TicketCreated(guild, author, message, option);
      var embedUserMessageSentToUser = ModmailEmbedBuilder.ToUser.MessageSent(guild, author, message);

      await channel.SendMessageAsync(x => {
        x.AddEmbed(embedTicketCreated);
        x.AddEmbed(embedUserMessageSentToUser);
      });


      var embedLog = ModmailEmbedBuilder.ToLog.TicketCreated(author, message, mailChannel, guild, ticket.Id);
      await logChannel.SendMessageAsync(embedLog);


      if (option.IsSensitiveLogging) {
        var dbMessageLog = UtilMapper.DiscordMessageToEntity(message, ticket.Id);
        await dbService.AddMessageLog(dbMessageLog);

        var embed3 = ModmailEmbedBuilder.ToLog.MessageSentByUser(author,
                                                                 message,
                                                                 channel,
                                                                 ticket.Id,
                                                                 guildId);
        await logChannel.SendMessageAsync(embed3);
      }
    }
    else {
      //continue on existing channel
      var mailChannel = guild.GetChannel(activeTicket.ModMessageChannelId);
      var embed = ModmailEmbedBuilder.ToMail.MessageReceived(author, message);
      await mailChannel.SendMessageAsync(embed);

      activeTicket.LastMessageDate = DateTime.Now;
      await dbService.UpdateTicketAsync(activeTicket);

      var embedUserMessageDelivered = ModmailEmbedBuilder.ToUser.MessageSent(guild, author, message);
      await channel.SendMessageAsync(embedUserMessageDelivered);

      if (option.IsSensitiveLogging) {
        var dbMessageLog = UtilMapper.DiscordMessageToEntity(message, activeTicket.Id);
        await dbService.AddMessageLog(dbMessageLog);

        var embed3 = ModmailEmbedBuilder.ToLog.MessageSentByUser(author,
                                                                 message,
                                                                 channel,
                                                                 activeTicket.Id,
                                                                 guildId);
        await logChannel.SendMessageAsync(embed3);
      }
    }
  }

  internal static async Task HandleGuildMessage(DiscordClient sender,
                                                DiscordMessage message,
                                                DiscordChannel channel,
                                                DiscordUser author,
                                                DiscordGuild guild) {
    if (message.Author.IsBot) return;
    if (message.IsTTS) return;
    if (channel.IsPrivate) return;
    if (guild is null) return;
    var channelId = channel.Id;
    var authorId = author.Id;
    var messageContent = message.Content;
    var attachments = message.Attachments;
    var guildId = guild.Id;
    if (message.Content.StartsWith(MMConfig.This.BotPrefix))
      //ignored
      return;

    var id = UtilChannelTopic.GetTicketIdFromChannelTopic(channel.Topic);
    if (id == Guid.Empty) {
      Log.Verbose("Failed to parse mail id from channel topic");
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var option = await dbService.GetOptionAsync(MMConfig.This.MainServerId);
    if (option is null) {
      Log.Error("Option not found for guild: {GuildOptionId}", guildId);
      return;
    }

    // await using var db = new ModmailDbContext();
    var ticket = await dbService.GetActiveTicketAsync(id);
    if (ticket is null) {
      Log.Error("Modmail not found for channel: {ChannelId}", channelId);
      return;
    }

    // var option = await db.GetOptionAsync(MMConfig.This.MainServerId);

    var ticketChannel = guild.GetChannel(ticket.ModMessageChannelId);
    if (ticketChannel is null) {
      Log.Error("Modmail channel not found for channel: {ChannelId}", channelId);
      return;
    }
    // var logChannel = ModmailBot.This.GetLogChannelAsync();


    var user = await guild.GetMemberAsync(ticket.DiscordUserId);
    var embed = ModmailEmbedBuilder.ToUser.MessageReceived(author, message, guild, ticket.Anonymous);
    await user.SendMessageAsync(embed);

    var embed2 = ModmailEmbedBuilder.ToMail.MessageSent(author, user, message, channel, ticket.Anonymous);
    await ticketChannel.SendMessageAsync(embed2);
    await message.DeleteAsync();

    ticket.LastMessageDate = DateTime.Now;
    await dbService.UpdateTicketAsync(ticket);


    if (option.IsSensitiveLogging) {
      var dbMessageLog = UtilMapper.DiscordMessageToEntity(message, ticket.Id);
      await dbService.AddMessageLog(dbMessageLog);


      var logChannelId = option.LogChannelId;
      var logChannel = guild.GetChannel(logChannelId);
      var embed3 = ModmailEmbedBuilder.ToLog.MessageSentByMod(author,
                                                              user,
                                                              message,
                                                              channel,
                                                              ticket.Id,
                                                              guildId,
                                                              ticket.Anonymous);
      await logChannel.SendMessageAsync(embed3);
    }
  }
}