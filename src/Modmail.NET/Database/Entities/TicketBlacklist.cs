using System.ComponentModel.DataAnnotations;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

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