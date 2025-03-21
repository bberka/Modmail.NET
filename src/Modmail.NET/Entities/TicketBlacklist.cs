﻿using System.ComponentModel.DataAnnotations;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public sealed class TicketBlacklist : IHasRegisterDate,
                                      IEntity
{
  public Guid Id { get; set; }

  [MaxLength(DbLength.REASON)]
  public string Reason { get; set; }

  public ulong DiscordUserId { get; set; }

  public DiscordUserInfo DiscordUser { get; set; }

  public DateTime RegisterDateUtc { get; set; }
}