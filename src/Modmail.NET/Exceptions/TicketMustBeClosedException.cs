namespace Modmail.NET.Exceptions;

public class TicketMustBeClosedException : BotExceptionBase
{
  public TicketMustBeClosedException() : base(LangProvider.This.GetTranslation(LangKeys.TICKET_MUST_BE_CLOSED)) { }
}