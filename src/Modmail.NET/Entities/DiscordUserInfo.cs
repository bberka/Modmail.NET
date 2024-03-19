using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;

namespace Modmail.NET.Entities;

public class DiscordUserInfo
{
  public DiscordUserInfo() {
    
  }
  public DiscordUserInfo(DiscordUser user) {
    Id = user.Id;
    Username = user.Username;
    AvatarUrl = user.AvatarUrl;
    BannerUrl = user.BannerUrl;
    Email = user.Email;
    Locale = user.Locale;
  }

  public DiscordUserInfo(DiscordMember member) {
    Id = member.Id;
    Username = member.Username;
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

  public DateTime RegisterDate { get; set; } = DateTime.Now;

  public DateTime? UpdateDate { get; set; }

  public string Username { get; set; }

  public string? AvatarUrl { get; set; }

  public string? BannerUrl { get; set; }

  public string Email { get; set; }

  public string Locale { get; set; }
}