using System.ComponentModel.DataAnnotations;
using Modmail.NET.Database.Abstract;
using Modmail.NET.Features.ActionLog.Static;

namespace Modmail.NET.Database.Entities;

public class ActionLog : IGuidId, IRegisterDateUtc, IEntity
{
    public ActionLogEvent Event { get; init; }
    public bool IsWebAction { get; init; }
    public string? Metadata { get; init; }

    [MaxLength(DbLength.IpAddress)]
    public string? IpAddress { get; init; }

    [MaxLength(DbLength.UserAgent)]
    public string? UserAgent { get; init; }

    public ulong UserId { get; init; }
    public virtual UserInformation? User { get; set; }
    public Guid Id { get; set; }
    public DateTime RegisterDateUtc { get; set; }
}