using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Modmail.NET.Common.Static;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database.Abstract;
using Modmail.NET.Features.Ticket.Static;

namespace Modmail.NET.Database.Entities;

public class Ticket : IHasRegisterDate,
                      IEntity,
                      IGuidId
{
  public DateTime LastMessageDateUtc { get; set; } = UtilDate.GetNow();
  public DateTime? ClosedDateUtc { get; set; }
  public required ulong OpenerUserId { get; set; } //FK
  public ulong? CloserUserId { get; set; } //FK
  public ulong? AssignedUserId { get; set; } //FK
  public required ulong PrivateMessageChannelId { get; set; }
  public required ulong ModMessageChannelId { get; set; }
  public required ulong InitialMessageId { get; set; }
  public required ulong BotTicketCreatedMessageInDmId { get; set; }
  public required TicketPriority Priority { get; set; }

  [MaxLength(DbLength.Reason)]
  public string CloseReason { get; set; }

  public bool IsForcedClosed { get; set; }
  public int? FeedbackStar { get; set; }

  [MaxLength(DbLength.FeedbackMessage)]
  public string FeedbackMessage { get; set; }

  public bool Anonymous { get; set; }

  [ForeignKey(nameof(TicketType))]
  public Guid? TicketTypeId { get; set; }

  //FK

  public DiscordUserInfo OpenerUser { get; set; }
  public DiscordUserInfo CloserUser { get; set; }
  public DiscordUserInfo AssignedUser { get; set; }
  public TicketType TicketType { get; set; }
  public List<TicketMessage> Messages { get; set; } = [];
  public List<TicketNote> TicketNotes { get; set; } = [];
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; }
}