using DSharpPlus.Entities;
using Modmail.NET.Entities;

namespace Modmail.NET.Common;

public static class UtilMapper
{
  public static TicketMessage DiscordMessageToEntity(DiscordMessage message, Guid ticketId) {
    var id = Guid.NewGuid();
    return new TicketMessage {
      Id = id,
      DiscordUserInfoId = message.Author.Id,
      MessageContent = message.Content,
      TicketId = ticketId,
      TicketMessageAttachments = message.Attachments.Select(x => DiscordAttachmentToEntity(x, id)).ToList(),
      MessageDiscordId = message.Id,
      RegisterDateUtc = DateTime.UtcNow,
    };
  }

  public static TicketMessageAttachment DiscordAttachmentToEntity(DiscordAttachment attachment, Guid ticketMessageId) {
    return new TicketMessageAttachment {
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