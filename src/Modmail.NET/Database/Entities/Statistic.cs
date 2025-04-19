using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class Statistic : IRegisterDateUtc,
                         IEntity,
                         IGuidId
{
	public Guid Id { get; set; }
	public DateTime RegisterDateUtc { get; set; }

	[Precision(2)]
	public required double AvgResponseTimeSeconds { get; init; }

	[Precision(2)]
	public required double AvgTicketsClosedPerDay { get; init; }

	[Precision(2)]
	public required double AvgTicketsOpenedPerDay { get; init; }

	[Precision(2)]
	public required double AvgTicketClosedSeconds { get; init; }

	[Precision(2)]
	public required double FastestClosedTicketSeconds { get; init; }

	[Precision(2)]
	public required double SlowestClosedTicketSeconds { get; init; }

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