namespace Modmail.NET.Exceptions;

public class TicketTimeoutOutOfRangeException : BotExceptionBase
{
  public TicketTimeoutOutOfRangeException() : base(LangKeys.TICKET_TIMEOUT_VALUE_IS_OUT_OF_RANGE.GetTranslation(),
                                                   LangKeys.TICKET_TIMEOUT_VALUE_MUST_BE_BETWEEN_X_AND_Y.GetTranslation(Const.TicketTimeoutMinAllowedHours,
                                                                                                                        Const.TicketTimeoutMaxAllowedHours)) { }
}