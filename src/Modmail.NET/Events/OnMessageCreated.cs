using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Metran;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Events;

public static class OnMessageCreated
{
  private static readonly MetranContainer<ulong> ProcessingUserMessageContainer = new();

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

    //keeps other threads for same user locked until this one is done
    using var metran = ProcessingUserMessageContainer.BeginTransaction(authorId, 50, 100); // 100ms * 50 = 5 seconds
    if (metran is null) {
      //VERY UNLIKELY TO HAPPEN
      await channel.SendMessageAsync(ModmailEmbeds.Base(Texts.SYSTEM_IS_BUSY, Texts.YOUR_MESSAGE_COULD_NOT_BE_PROCESSED, DiscordColor.DarkRed));
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();
    //Check if user has active modmail
    // await using var db = new ModmailDbContext();
    var option = await dbService.GetOptionAsync(MMConfig.This.MainServerId);
    if (option is null) {
      Log.Error("Option not found for guild: {GuildOptionId}", guildId);
      return;
    }

    var guild = await sender.GetGuildAsync(option.GuildId);

    var dcUserInfo = new DiscordUserInfo(author);
    await dbService.UpdateUserInfoAsync(dcUserInfo);

    var activeBlock = await dbService.GetUserBlacklistStatus(authorId);
    if (activeBlock) {
      var embed = ModmailEmbeds.ToUser.UserBlocked(author, guild);
      await channel.SendMessageAsync(embed);
      return;
    }

    var activeTicket = await dbService.GetActiveTicketAsync(authorId);
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
        if (role.Key != 0) {
          var exists = roleListForOverwrites.Any(x => x.Id == role.Key);
          if (!exists)
            roleListForOverwrites.Add(role.Value);
        }

        var member2 = members.FirstOrDefault(x => x.Id == perm.Key && perm.Type == TeamMemberDataType.UserId);
        if (member2 is not null && member2.Id != 0) {
          var exists = memberListForOverwrites.Any(x => x.Id == member2.Id);
          if (!exists)
            memberListForOverwrites.Add(member2);
        }
      }


      var permissionOverwrites = UtilPermission.GetTicketPermissionOverwrites(guild, memberListForOverwrites, roleListForOverwrites);
      var mailChannel = await guild.CreateTextChannelAsync(channelName, category, UtilChannelTopic.BuildChannelTopic(ticketId), permissionOverwrites);

      var member = await guild.GetMemberAsync(author.Id);
      var embedNewTicket = ModmailEmbeds.ToMail.NewTicket(member);
      var sb = new StringBuilder();
      if (roleListForOverwrites.Count > 0) {
        sb.AppendLine(Texts.ROLES + ":");
        foreach (var role in roleListForOverwrites) sb.AppendLine(role.Mention);
      }

      if (memberListForOverwrites.Count > 0) {
        sb.AppendLine(Texts.MEMBERS + ":");
        foreach (var member2 in memberListForOverwrites) sb.AppendLine(member2.Mention);
      }


      await mailChannel.SendMessageAsync(sb.ToString(), embedNewTicket);

      var embedUserMessage = ModmailEmbeds.ToMail.MessageReceived(author, message);
      await mailChannel.SendMessageAsync(embedUserMessage);


      var ticket = new Ticket {
        DiscordUserInfoId = authorId,
        ModMessageChannelId = mailChannel.Id,
        RegisterDateUtc = DateTime.UtcNow,
        PrivateMessageChannelId = channelId,
        InitialMessageId = message.Id,
        Priority = TicketPriority.Normal,
        LastMessageDateUtc = DateTime.UtcNow,
        GuildOptionId = guildId,
        Id = ticketId,
        Anonymous = false,
        IsForcedClosed = false
      };

      await dbService.AddTicketAsync(ticket);


      var embedTicketCreated = ModmailEmbeds.ToUser.TicketCreated(guild, author, message, option);
      var embedUserMessageSentToUser = ModmailEmbeds.ToUser.MessageSent(guild, author, message);

      await channel.SendMessageAsync(x => {
        x.AddEmbed(embedTicketCreated);
        x.AddEmbed(embedUserMessageSentToUser);
      });


      var embedLog = ModmailEmbeds.ToLog.TicketCreated(author, message, mailChannel, guild, ticket.Id);
      await logChannel.SendMessageAsync(embedLog);


      if (option.IsSensitiveLogging) {
        var dbMessageLog = UtilMapper.DiscordMessageToEntity(message, ticket.Id);
        await dbService.AddMessageLog(dbMessageLog);

        var embed3 = ModmailEmbeds.ToLog.MessageSentByUser(author,
                                                           message,
                                                           channel,
                                                           ticket.Id,
                                                           guildId);
        await logChannel.SendMessageAsync(embed3);
      }
    }
    else {
      //continue on existing channel
      await Task.Delay(70); //wait for channel creation process to finish
      var mailChannel = guild.GetChannel(activeTicket.ModMessageChannelId);
      var embed = ModmailEmbeds.ToMail.MessageReceived(author, message);
      await mailChannel.SendMessageAsync(embed);

      activeTicket.LastMessageDateUtc = DateTime.UtcNow;
      await dbService.UpdateTicketAsync(activeTicket);

      var embedUserMessageDelivered = ModmailEmbeds.ToUser.MessageSent(guild, author, message);
      await channel.SendMessageAsync(embedUserMessageDelivered);

      if (option.IsSensitiveLogging) {
        var dbMessageLog = UtilMapper.DiscordMessageToEntity(message, activeTicket.Id);
        await dbService.AddMessageLog(dbMessageLog);

        var embed3 = ModmailEmbeds.ToLog.MessageSentByUser(author,
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
                                                DiscordUser modUser,
                                                DiscordGuild guild) {
    if (message.Author.IsBot) return;
    if (message.IsTTS) return;
    if (channel.IsPrivate) return;
    if (guild is null) return;
    var channelId = channel.Id;
    var authorId = modUser.Id;
    var messageContent = message.Content;
    var attachments = message.Attachments;
    var guildId = guild.Id;
    if (message.Content.StartsWith(MMConfig.This.BotPrefix))
      //ignored
      return;

    using var metran = ProcessingUserMessageContainer.BeginTransaction(authorId, 50, 100); // 100ms * 50 = 5 seconds
    if (metran is null) {
      //VERY UNLIKELY TO HAPPEN
      await channel.SendMessageAsync(ModmailEmbeds.Base(Texts.SYSTEM_IS_BUSY, Texts.YOUR_MESSAGE_COULD_NOT_BE_PROCESSED, DiscordColor.DarkRed));
      return;
    }

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


    var dcUserInfo = new DiscordUserInfo(modUser);
    await dbService.UpdateUserInfoAsync(dcUserInfo);

    var user = await guild.GetMemberAsync(ticket.DiscordUserInfoId);
    var embed = ModmailEmbeds.ToUser.MessageReceived(modUser, message, guild, ticket.Anonymous);
    await user.SendMessageAsync(embed);

    var embed2 = ModmailEmbeds.ToMail.MessageSent(modUser, user, message, channel, ticket.Anonymous);
    await ticketChannel.SendMessageAsync(embed2);
    await message.DeleteAsync();

    ticket.LastMessageDateUtc = DateTime.UtcNow;
    await dbService.UpdateTicketAsync(ticket);


    if (option.IsSensitiveLogging) {
      var dbMessageLog = UtilMapper.DiscordMessageToEntity(message, ticket.Id);
      await dbService.AddMessageLog(dbMessageLog);


      var logChannelId = option.LogChannelId;
      var logChannel = guild.GetChannel(logChannelId);
      var embed3 = ModmailEmbeds.ToLog.MessageSentByMod(modUser,
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