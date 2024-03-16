using System.ComponentModel.DataAnnotations;
using Modmail.NET.Static;

namespace Modmail.NET.Entities;

public class Ticket
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDate { get; set; }
  public DateTime LastMessageDate { get; set; }
  public DateTime? ClosedDate { get; set; }

  public ulong DiscordUserId { get; set; }

  public ulong PrivateMessageChannelId { get; set; }


  public ulong ModMessageChannelId { get; set; }


  public ulong GuildId { get; set; }

  public ulong InitialMessageId { get; set; }

  public TicketPriority Priority { get; set; }

  public bool IsForcedClosed { get; set; } = false;
  //FK

  public virtual List<TicketMessage> TicketMessages { get; set; }
}