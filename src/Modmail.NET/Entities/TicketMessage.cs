﻿using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Modmail.NET.Abstract;
using Modmail.NET.Utils;

namespace Modmail.NET.Entities;

public sealed class TicketMessage : IHasRegisterDate,
                                    IEntity
{
  public Guid Id { get; set; }
  public ulong SenderUserId { get; set; }

  [MaxLength(DbLength.MESSAGE)]
  [Required]
  public required string MessageContent { get; set; }

  public ulong MessageDiscordId { get; set; }
  public Guid TicketId { get; set; }

  //FK
  public List<TicketMessageAttachment> Attachments { get; set; }
  public DateTime RegisterDateUtc { get; set; }

  public static TicketMessage MapFrom(Guid ticketId, DiscordMessage message) {
    var id = Guid.NewGuid();
    return new TicketMessage {
      Id = id,
      SenderUserId = message.Author.Id,
      MessageContent = message.Content,
      TicketId = ticketId,
      Attachments = message.Attachments.Select(x => TicketMessageAttachment.MapFrom(x, id)).ToList(),
      MessageDiscordId = message.Id,
      RegisterDateUtc = DateTime.UtcNow
    };
  }

  public static TicketMessage MapFrom(Guid ticketId, ulong authorId, ulong messageId, string messageContent, List<DiscordAttachment> discordAttachments) {
    discordAttachments ??= new List<DiscordAttachment>();
    var id = Guid.NewGuid();
    return new TicketMessage {
      Id = id,
      SenderUserId = authorId,
      MessageContent = messageContent,
      TicketId = ticketId,
      Attachments = discordAttachments.Select(x => TicketMessageAttachment.MapFrom(x, id)).ToList(),
      MessageDiscordId = messageId,
      RegisterDateUtc = UtilDate.GetNow()
    };
  }
}