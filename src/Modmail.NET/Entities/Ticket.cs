using System.ComponentModel.DataAnnotations;
using Modmail.NET.Static;

namespace Modmail.NET.Entities;

public class Ticket
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime LastMessageDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime? ClosedDateUtc { get; set; }

  public ulong DiscordUserInfoId { get; set; } //FK

  public ulong PrivateMessageChannelId { get; set; }


  public ulong ModMessageChannelId { get; set; }


  public ulong InitialMessageId { get; set; }

  public TicketPriority Priority { get; set; }

  public string? CloseReason { get; set; }
  public bool IsForcedClosed { get; set; } = false;

  public ulong GuildOptionId { get; set; }
  public int? FeedbackStar { get; set; }
  public string? FeedbackMessage { get; set; }
  public bool Anonymous { get; set; }
  //FK

  public virtual DiscordUserInfo DiscordUserInfo { get; set; }

  public virtual GuildOption GuildOption { get; set; }
  public virtual List<TicketMessage> TicketMessages { get; set; }

  public virtual List<TicketNote> TicketNotes { get; set; }
}