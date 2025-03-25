using System.ComponentModel.DataAnnotations;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public class TicketBlacklist : IHasRegisterDate,
                               IEntity,
                               IGuidId
{
  [MaxLength(DbLength.Reason)]
  public required string Reason { get; set; }

  public required ulong DiscordUserId { get; set; }

  public DiscordUserInfo DiscordUser { get; set; }
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; }
}