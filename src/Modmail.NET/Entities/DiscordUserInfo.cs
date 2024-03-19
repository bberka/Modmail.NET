using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Modmail.NET.Common;

namespace Modmail.NET.Entities;

public class DiscordUserInfo
{
  public DiscordUserInfo() {
    
  }
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
  /// Users Discord Id
  /// </summary>
  [Key]
  public ulong Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;

  public DateTime? UpdateDateUtc { get; set; }

  public string Username { get; set; }

  public string? AvatarUrl { get; set; }

  public string? BannerUrl { get; set; }

  public string? Email { get; set; }

  public string? Locale { get; set; }
}