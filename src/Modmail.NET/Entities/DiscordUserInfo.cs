using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Modmail.NET.Abstract;
using Modmail.NET.Extensions;

namespace Modmail.NET.Entities;

public class DiscordUserInfo : IHasRegisterDate,
                               IHasUpdateDate,
                               IEntity
{
  public DiscordUserInfo() { }

  public DiscordUserInfo(DiscordUser user) {
    Id = user.Id;
    Username = user.GetUsername();
    AvatarUrl = user.AvatarUrl;
    BannerUrl = user.BannerUrl;
    Email = user.Email;
    Locale = user.Locale;
  }

  public DiscordUserInfo(DiscordMember member) {
    Id = member.Id;
    Username = member.GetUsername();
    AvatarUrl = member.AvatarUrl;
    BannerUrl = member.BannerUrl;
    Email = member.Email;
    Locale = member.Locale;
  }

  /// <summary>
  ///   Users Discord Id
  /// </summary>
  public ulong Id { get; set; }

  [MaxLength(DbLength.Name)]
  [Required]
  public required string Username { get; set; }

  [MaxLength(DbLength.Url)]
  public string AvatarUrl { get; set; }

  [MaxLength(DbLength.Url)]
  public string BannerUrl { get; set; }

  [MaxLength(DbLength.Email)]
  public string Email { get; set; }

  [MaxLength(DbLength.Locale)]
  public string Locale { get; set; } = string.Empty;

  public List<Ticket> OpenedTickets { get; set; } = [];
  public List<Ticket> ClosedTickets { get; set; } = [];
  public List<Ticket> AssignedTickets { get; set; } = [];

  public DateTime RegisterDateUtc { get; set; }

  public DateTime? UpdateDateUtc { get; set; }

  public string GetMention() {
    return $"<@{Id}>";
  }
}