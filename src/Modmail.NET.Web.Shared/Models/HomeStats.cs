namespace Modmail.NET.Web.Shared.Models;

public sealed record HomeStats(
  int ActiveTickets,
  int ClosedTickets,
  int TotalMessages,
  int Teams,
  int Blacklist,
  int TicketTypes,
  int TeamMemberCount,
  int TeamRoleCount,
  int ProcessingQueueCount
  );