using System.ComponentModel.DataAnnotations;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public sealed class TicketNote : IHasRegisterDate,
                                 IEntity
{
  public Guid Id { get; set; }

  [MaxLength(DbLength.NOTE)]
  [Required]
  public required string Content { get; set; }

  public Guid TicketId { get; set; }
  public ulong DiscordUserId { get; set; }
  public DateTime RegisterDateUtc { get; set; }
}