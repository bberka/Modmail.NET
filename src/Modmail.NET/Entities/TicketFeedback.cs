using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class TicketFeedback
{
  [Key]
  public Guid Id { get; set; }
 
  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;

  public byte Stars { get; set; }

  public string? Content { get; set; }


  public Guid TicketId { get; set; }
  public virtual Ticket Ticket { get; set; }
}