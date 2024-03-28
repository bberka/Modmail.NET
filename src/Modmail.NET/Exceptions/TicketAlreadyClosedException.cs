using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class TicketAlreadyClosedException : BotExceptionBase
{
  public TicketAlreadyClosedException() : base(Texts.TICKET_ALREADY_CLOSED) { }
}