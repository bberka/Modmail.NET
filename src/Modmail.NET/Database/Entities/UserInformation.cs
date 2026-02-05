using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class UserInformation : IRegisterDateUtc, IUpdateDateUtc, IUlongId, IEntity
{
    [MaxLength(DbLength.Name)]
    public string Username { get; set; } = null!;

    [MaxLength(DbLength.Url)]
    public string? AvatarUrl { get; set; }

    [MaxLength(DbLength.Url)]
    public string? BannerUrl { get; set; }

    [MaxLength(DbLength.Email)]
    public string? Email { get; set; }

    [MaxLength(DbLength.Locale)]
    public string? Locale { get; set; }

    public virtual ICollection<Ticket> OpenedTickets { get; set; } = [];
    public virtual ICollection<Ticket> ClosedTickets { get; set; } = [];
    public virtual ICollection<Ticket> AssignedTickets { get; set; } = [];

    public DateTime RegisterDateUtc { get; set; }

    /// <summary>
    ///     Users Discord Id
    /// </summary>
    [Range(1, long.MaxValue)]
    public ulong Id { get; set; }

    public DateTime? UpdateDateUtc { get; set; }

    public static UserInformation FromDiscordUser(DiscordUser user)
    {
        return new UserInformation
        {
            Id = user.Id,
            Username = user.GetUsername(),
            AvatarUrl = user.AvatarUrl,
            BannerUrl = user.BannerUrl,
            Email = user.Email,
            Locale = user.Locale
        };
    }

    public static UserInformation FromDiscordMember(DiscordMember member)
    {
        return new UserInformation
        {
            Id = member.Id,
            Username = member.GetUsername(),
            AvatarUrl = member.AvatarUrl,
            BannerUrl = member.BannerUrl,
            Email = member.Email,
            Locale = member.Locale
        };
    }

    public string GetMention()
    {
        return $"<@{Id}>";
    }
}