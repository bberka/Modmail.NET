namespace Modmail.NET.Models.Dto;

public sealed record HomeStatDto(
  int ActiveTickets,
  int ClosedTickets,
  int TotalMessages,
  int Teams,
  int Blacklist,
  int TicketTypes,
  int TeamMemberCount,
  int TeamRoleCount,
  int ProcessingQueueCount,
  double AvgResponseTimeMinutes,
  double AvgTicketsOpenPerDay,
  double AvgTicketsClosePerDay
);