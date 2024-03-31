namespace Modmail.NET.Exceptions;

public class TicketAlreadyClosedException : BotExceptionBase
{
  public TicketAlreadyClosedException() : base(LangData.This.GetTranslation(LangKeys.TICKET_ALREADY_CLOSED)) { }
}