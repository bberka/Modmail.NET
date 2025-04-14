using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Static;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database.Abstract;
using Modmail.NET.Features.Ticket.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Database.Entities;

public class TicketMessage : IRegisterDateUtc,
                             IGuidId,
                             IEntity
{
  public ulong BotMessageId { get; set; }
  public required ulong SenderUserId { get; set; }

  [MaxLength(DbLength.Message)]
  public string? MessageContent { get; set; }

  public required ulong MessageDiscordId { get; set; }
  public required Guid TicketId { get; set; }
  public required bool SentByMod { get; set; }
  public Guid? TagId { get; set; }

  public TicketMessageChangeStatus ChangeStatus { get; set; } = TicketMessageChangeStatus.None;

  //FK
  public virtual ICollection<TicketMessageAttachment> Attachments { get; set; } = [];
  public virtual ICollection<TicketMessageHistory> History { get; set; } = [];
  public virtual Tag? Tag { get; set; }
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; }

  public static TicketMessage MapFrom(Guid ticketId, DiscordMessage message, bool sentByMod) {
    var id = Guid.CreateVersion7();
    return new TicketMessage {
      Id = id,
      SenderUserId = message.Author?.Id ?? throw new ModmailBotException(Lang.InvalidUser),
      MessageContent = message.Content,
      TicketId = ticketId,
      Attachments = message.Attachments.Select(x => TicketMessageAttachment.MapFrom(x, id)).ToList(),
      MessageDiscordId = message.Id,
      RegisterDateUtc = UtilDate.GetNow(),
      SentByMod = sentByMod
    };
  }
}