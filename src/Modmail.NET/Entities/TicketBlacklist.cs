using System.ComponentModel.DataAnnotations;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public class TicketBlacklist : IHasRegisterDate,
                               IEntity,
                               IGuidId
{
  [MaxLength(DbLength.Reason)]
  public string Reason { get; set; }

  public ulong DiscordUserId { get; set; }

  public DiscordUserInfo DiscordUser { get; set; }
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; }
}