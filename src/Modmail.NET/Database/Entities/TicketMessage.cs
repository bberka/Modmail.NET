using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Static;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database.Abstract;
using Modmail.NET.Features.Ticket.Static;

namespace Modmail.NET.Database.Entities;

public class TicketMessage : IHasRegisterDate,
                             IEntity
{
  public Guid Id { get; set; }
  public required ulong SenderUserId { get; set; }

  [MaxLength(DbLength.Message)]
  public string MessageContent { get; set; }

  public required ulong MessageDiscordId { get; set; }
  public required Guid TicketId { get; set; }

  public required bool SentByMod { get; set; }

  public TicketMessageChangeStatus ChangeStatus { get; set; } = TicketMessageChangeStatus.None;

  //FK
  public virtual List<TicketMessageAttachment> Attachments { get; set; }
  public virtual List<TicketMessageHistory> History { get; set; }

  public ulong BotMessageId { get; set; }
  public DateTime RegisterDateUtc { get; set; }

  public static TicketMessage MapFrom(Guid ticketId, DiscordMessage message, bool sentByMod) {
    var id = Guid.CreateVersion7();
    return new TicketMessage {
      Id = id,
      SenderUserId = message.Author?.Id ?? throw new InvalidUserIdException(),
      MessageContent = message.Content,
      TicketId = ticketId,
      Attachments = message.Attachments.Select(x => TicketMessageAttachment.MapFrom(x, id)).ToList(),
      MessageDiscordId = message.Id,
      RegisterDateUtc = UtilDate.GetNow(),
      SentByMod = sentByMod
    };
  }

  public static TicketMessage MapFrom(Guid ticketId, ulong authorId, ulong messageId, string messageContent, List<DiscordAttachment> discordAttachments, bool sentByMod) {
    discordAttachments ??= [];
    var id = Guid.CreateVersion7();
    return new TicketMessage {
      Id = id,
      SenderUserId = authorId,
      MessageContent = messageContent,
      TicketId = ticketId,
      Attachments = discordAttachments.Select(x => TicketMessageAttachment.MapFrom(x, id)).ToList(),
      MessageDiscordId = messageId,
      RegisterDateUtc = UtilDate.GetNow(),
      SentByMod = sentByMod
    };
  }
}