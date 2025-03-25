using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class TicketTimeoutOutOfRangeException : BotExceptionBase
{
  public TicketTimeoutOutOfRangeException() : base(LangKeys.TicketTimeoutValueIsOutOfRange.GetTranslation(),
                                                   LangKeys.TicketTimeoutValueMustBeBetweenXAndY.GetTranslation(Const.TicketTimeoutMinAllowedHours,
                                                                                                                Const.TicketTimeoutMaxAllowedHours)) { }
}