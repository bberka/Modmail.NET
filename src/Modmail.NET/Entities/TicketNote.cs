using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class TicketNote
{
  [Key]
  public Guid Id { get; set; }
  public DateTime RegisterDate { get; set; } = DateTime.Now;
  public string Content { get; set; }
  public Guid TicketId { get; set; }
  public ulong UserId { get; set; }
  public string Username { get; set; }
  public virtual Ticket Ticket { get; set; }
}