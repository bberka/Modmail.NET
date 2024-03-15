using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Events;

public static class MessageEventHandlers
{
  public static async Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs args) {
    await Task.WhenAll(HandlePrivateMessage(sender, args.Message, args.Channel, args.Author),
                       HandleGuildMessage(sender, args.Message, args.Channel, args.Author, args.Guild));
  }

  public static async Task HandlePrivateMessage(DiscordClient sender,
                                                DiscordMessage message,
                                                DiscordChannel channel,
                                                DiscordUser author) {
    if (message.Author.IsBot) return;
    if (message.IsTTS) return;
    if (!channel.IsPrivate) return;
    var channelId = channel.Id;
    var authorId = author.Id;

    //Check if user has active modmail
    await using var db = new ModmailDbContext();
    var option = await db.GetOptionAsync(MMConfig.This.MainServerId);
    var activeMail = await db.GetActiveModmailAsync(authorId);
    var guild = await sender.GetGuildAsync(option.GuildId);
    var logChannel = guild.GetChannel(option.LogChannelId);


    if (activeMail is null) {
      //make new channel
      var channelName = $"modmail-{author.Username.Trim()}";
      var category = guild.GetChannel(option.CategoryId);


      var mailChannel = await guild.CreateTextChannelAsync(channelName, category);
      var embedUserMessage = ModmailEmbedBuilder.ToMail.MessageReceived(author, message);
      await mailChannel.SendMessageAsync(embedUserMessage);


      var ticket = new Ticket {
        DiscordUserId = authorId,
        ModMessageChannelId = mailChannel.Id,
        GuildId = guild.Id,
        RegisterDate = DateTime.Now,
        PrivateMessageChannelId = channelId,
        InitialMessageId = message.Id,
        Priority = MailPriority.Normal,
        LastMessageDate = DateTime.Now
      };

      db.Add(ticket);
      await db.SaveChangesAsync();

      await mailChannel.ModifyAsync(x => { x.Topic = UtilChannelTopic.BuildChannelTopic(ticket.Id); });

      var embedUserMessageDelivered = ModmailEmbedBuilder.ToUser.TicketCreated(guild, author, message);
      await channel.SendMessageAsync(embedUserMessageDelivered);

      var embedLog = ModmailEmbedBuilder.ToLog.TicketCreated(author, message, mailChannel, ticket.Id);
      await logChannel.SendMessageAsync(embedLog);
    }
    else {
      //continue on existing channel
      var mailChannel = guild.GetChannel(activeMail.ModMessageChannelId);
      var embed = ModmailEmbedBuilder.ToMail.MessageReceived(author, message);
      await mailChannel.SendMessageAsync(embed);

      activeMail.LastMessageDate = DateTime.Now;
      db.Update(activeMail);
      await db.SaveChangesAsync();

      var embedUserMessageDelivered = ModmailEmbedBuilder.ToUser.MessageSent(guild, author, message);
      await channel.SendMessageAsync(embedUserMessageDelivered);
    }
  }

  public static async Task HandleGuildMessage(DiscordClient sender,
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
    var id = UtilChannelTopic.GetTicketIdFromChannelTopic(channel.Topic);
    if (id == Guid.Empty) {
      Log.Error("Failed to parse mail id from channel topic");
      return;
    }

    await using var db = new ModmailDbContext();
    var ticket = await db.GetActiveModmailAsync(id);
    if (ticket is null) {
      Log.Error("Modmail not found for channel: {ChannelId}", channelId);
      return;
    }

    // var option = await db.GetOptionAsync(MMConfig.This.MainServerId);

    var modmailChannel = guild.GetChannel(ticket.ModMessageChannelId);
    if (modmailChannel is null) {
      Log.Error("Modmail channel not found for channel: {ChannelId}", channelId);
      return;
    }
    // var logChannel = ModmailBot.This.GetLogChannelAsync();


    var user = await guild.GetMemberAsync(authorId);
    var embed = ModmailEmbedBuilder.ToUser.MessageReceived(author, message, guild);
    await user.SendMessageAsync(embed);

    var embed2 = ModmailEmbedBuilder.ToMail.MessageSent(author, user, message, channel);
    await modmailChannel.SendMessageAsync(embed2);
    await message.DeleteAsync();

    ticket.LastMessageDate = DateTime.Now;
    db.Update(ticket);

    var dbMessageLog = UtilMapper.DiscordMessageToEntity(message, ticket.Id, guildId);
    db.Add(dbMessageLog);

    await db.SaveChangesAsync();
  }

}