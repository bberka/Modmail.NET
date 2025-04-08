using Modmail.NET.Features.Ticket.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class TicketTimeoutOutOfRangeException : ModmailBotException
{
  public TicketTimeoutOutOfRangeException() : base(LangKeys.TicketTimeoutValueIsOutOfRange.GetTranslation(),
                                                   LangKeys.TicketTimeoutValueMustBeBetweenXAndY.GetTranslation(TicketConstants.TicketTimeoutMinAllowedHours,
                                                                                                                TicketConstants.TicketTimeoutMaxAllowedHours)) { }
}