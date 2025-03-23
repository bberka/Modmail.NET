using Microsoft.EntityFrameworkCore;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public class Statistic : IHasRegisterDate,
                         IEntity
{
  public Guid Id { get; set; }

  [Precision(2)]
  public double AvgResponseTimeMinutes { get; set; }

  [Precision(2)]
  public double AvgTicketsClosedPerDay { get; set; }

  [Precision(2)]
  public double AvgTicketsOpenedPerDay { get; set; }

  [Precision(2)]
  public double AvgTicketResolvedMinutes { get; set; }

  [Precision(2)]
  public double FastestClosedTicketMinutes { get; set; }

  [Precision(2)]
  public double SlowestClosedTicketMinutes { get; set; }

  public DateTime RegisterDateUtc { get; set; }
}