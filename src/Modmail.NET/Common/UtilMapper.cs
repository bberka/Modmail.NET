using DSharpPlus.Entities;
using Modmail.NET.Entities;

namespace Modmail.NET.Common;

public static class UtilMapper
{
  public static TicketMessage DiscordMessageToEntity(DiscordMessage message, Guid ticketId, ulong guildId) {
    var id = Guid.NewGuid();
    return new TicketMessage {
      Id = id,
      AuthorId = message.Author.Id,
      Discriminator = message.Author.Discriminator,
      Username = message.Author.Username,
      MessageContent = message.Content,
      GuildId = message.Channel.GuildId ?? guildId,
      ChannelId = message.ChannelId,
      MessageId = message.Id,
      TicketId = ticketId,
      TicketMessageAttachments = message.Attachments.Select(x => DiscordAttachmentToEntity(x, id)).ToList()
    };
  }

  public static TicketMessageAttachment DiscordAttachmentToEntity(DiscordAttachment attachment, Guid ticketMessageId) {
    return new TicketMessageAttachment {
      RegisterDate = DateTime.Now,
      Url = attachment.Url,
      ProxyUrl = attachment.ProxyUrl,
      TicketMessageId = ticketMessageId,
      Height = attachment.Height,
      Width = attachment.Width,
      FileName = attachment.FileName,
      FileSize = attachment.FileSize,
      MediaType = attachment.MediaType
    };
  }
}