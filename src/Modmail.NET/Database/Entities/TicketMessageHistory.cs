using System.ComponentModel.DataAnnotations;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class TicketMessageHistory : IRegisterDateUtc, IGuidId, IEntity
{
    [MaxLength(DbLength.Message)]
    public required string? MessageContentBefore { get; init; }

    [MaxLength(DbLength.Message)]
    public required string? MessageContentAfter { get; init; }

    public Guid TicketMessageId { get; init; }
    public virtual TicketMessage? TicketMessage { get; set; }
    public Guid Id { get; set; }
    public DateTime RegisterDateUtc { get; set; }
}