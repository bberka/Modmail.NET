using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Metran;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;
using Modmail.NET.Utils;

namespace Modmail.NET.Events;

public static class OnMessageCreated
{
  private static readonly MetranContainer<ulong> ProcessingUserMessageContainer = new();

  public static async Task Handle(DiscordClient sender, MessageCreateEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.Author);
    if (args.Message.Author.IsBot) return;
    if (args.Message.IsTTS) return;
    if (args.Channel.IsPrivate) await HandlePrivateTicketMessageAsync(sender, args.Message, args.Channel, args.Author);
    else await HandleGuildTicketMessageAsync(sender, args.Message, args.Channel, args.Author, args.Guild);
  }

  internal static async Task HandlePrivateTicketMessageAsync(DiscordClient sender,
                                                             DiscordMessage message,
                                                             DiscordChannel channel,
                                                             DiscordUser user) {
    var userId = user.Id;
    if (message.Content.StartsWith(BotConfig.This.BotPrefix))
      return;

    //keeps other threads for same user locked until this one is done
    using var metran = ProcessingUserMessageContainer.BeginTransaction(userId, 50, 100); // 100ms * 50 = 5 seconds
    if (metran is null) {
      //VERY UNLIKELY TO HAPPEN
      await channel.SendMessageAsync(Embeds.Error(Texts.SYSTEM_IS_BUSY, Texts.YOUR_MESSAGE_COULD_NOT_BE_PROCESSED));
      return;
    }

    var activeBlock = await TicketBlacklist.IsBlacklistedAsync(userId);
    if (activeBlock) {
      await channel.SendMessageAsync(EmbedUser.YouHaveBeenBlacklisted());
      return;
    }

    var activeTicket = await Ticket.GetActiveAsync(userId);
    if (activeTicket is not null) {
      await activeTicket.ProcessUserSentMessageAsync(message, channel);
    }
    else {
      await Ticket.ProcessCreateNewTicketAsync(user, channel, message);
    }
  }

  internal static async Task HandleGuildTicketMessageAsync(DiscordClient sender,
                                                           DiscordMessage message,
                                                           DiscordChannel channel,
                                                           DiscordUser modUser,
                                                           DiscordGuild guild) {
    var channelId = channel.Id;
    var authorId = modUser.Id;
    var messageContent = message.Content;
    var attachments = message.Attachments;
    var guildId = guild.Id;
    if (message.Content.StartsWith(BotConfig.This.BotPrefix))
      return;

    using var metran = ProcessingUserMessageContainer.BeginTransaction(authorId, 50, 100); // 100ms * 50 = 5 seconds
    if (metran is null) {
      await channel.SendMessageAsync(Embeds.Error(Texts.SYSTEM_IS_BUSY, Texts.YOUR_MESSAGE_COULD_NOT_BE_PROCESSED));
      return;
    }

    var id = UtilChannelTopic.GetTicketIdFromChannelTopic(channel.Topic);
    if (id == Guid.Empty) return;

    var ticket = await Ticket.GetActiveAsync(id);
    if (ticket is null) return;

    await ticket.ProcessModSendMessageAsync(modUser, message, channel, guild);
  }
}