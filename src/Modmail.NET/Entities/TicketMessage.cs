using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class TicketMessage
{
  [Key]
  public Guid Id { get; set; }

  public ulong AuthorId { get; set; }

  public string Discriminator { get; set; }

  public string Username { get; set; }

  public string MessageContent { get; set; }


  //FK
  public virtual Guid TicketId { get; set; }
  public virtual List<TicketMessageAttachment> TicketMessageAttachments { get; set; }
}