using Microsoft.EntityFrameworkCore;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public class Statistic : IHasRegisterDate,
                         IEntity,
                         IGuidId
{
  [Precision(2)]
  public required double AvgResponseTimeMinutes { get; set; }

  [Precision(2)]
  public required double AvgTicketsClosedPerDay { get; set; }

  [Precision(2)]
  public required double AvgTicketsOpenedPerDay { get; set; }

  [Precision(2)]
  public required double AvgTicketResolvedMinutes { get; set; }

  [Precision(2)]
  public required double FastestClosedTicketMinutes { get; set; }

  [Precision(2)]
  public required double SlowestClosedTicketMinutes { get; set; }

  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; }

  public static Statistic Default() {
    return new Statistic {
      AvgResponseTimeMinutes = 0,
      AvgTicketsClosedPerDay = 0,
      AvgTicketsOpenedPerDay = 0,
      AvgTicketResolvedMinutes = 0,
      FastestClosedTicketMinutes = 0,
      SlowestClosedTicketMinutes = 0,
    };
  }
}