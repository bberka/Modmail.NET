using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class TicketAlreadyClosedException : BotExceptionBase
{
  public TicketAlreadyClosedException() : base(LangProvider.This.GetTranslation(LangKeys.TICKET_ALREADY_CLOSED)) { }
}