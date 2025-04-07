using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class Statistic : IHasRegisterDate,
                         IEntity,
                         IGuidId
{
  [Precision(2)]
  public required double AvgResponseTimeSeconds { get; set; }

  [Precision(2)]
  public required double AvgTicketsClosedPerDay { get; set; }

  [Precision(2)]
  public required double AvgTicketsOpenedPerDay { get; set; }

  [Precision(2)]
  public required double AvgTicketClosedSeconds { get; set; }

  [Precision(2)]
  public required double FastestClosedTicketSeconds { get; set; }

  [Precision(2)]
  public required double SlowestClosedTicketSeconds { get; set; }

  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; }

  public static Statistic Default() {
    return new Statistic {
      AvgResponseTimeSeconds = 0,
      AvgTicketsClosedPerDay = 0,
      AvgTicketsOpenedPerDay = 0,
      AvgTicketClosedSeconds = 0,
      FastestClosedTicketSeconds = 0,
      SlowestClosedTicketSeconds = 0
    };
  }
}