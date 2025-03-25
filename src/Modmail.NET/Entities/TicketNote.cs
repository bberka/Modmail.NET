using System.ComponentModel.DataAnnotations;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public class TicketNote : IHasRegisterDate,
                          IEntity,
                          IGuidId
{
  [MaxLength(DbLength.Note)]
  [Required]
  public required string Content { get; set; }

  public Guid TicketId { get; set; }
  public ulong DiscordUserId { get; set; }
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; }
}