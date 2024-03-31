namespace Modmail.NET.Exceptions;

public class TicketNotFoundException : BotExceptionBase
{
  public TicketNotFoundException() : base(LangData.This.GetTranslation(LangKeys.TICKET_NOT_FOUND)) { }
}