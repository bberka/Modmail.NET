using System.ComponentModel.DataAnnotations;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class TicketMessageHistory : IHasRegisterDate,
                                    IEntity
{
  public Guid Id { get; set; }

  [MaxLength(DbLength.Message)]
  public required string MessageContentBefore { get; set; } = null;

  [MaxLength(DbLength.Message)]
  public required string MessageContentAfter { get; set; } = null;

  public required Guid TicketMessageId { get; set; }

  //FK
  public virtual TicketMessage TicketMessage { get; set; }
  public DateTime RegisterDateUtc { get; set; }
}