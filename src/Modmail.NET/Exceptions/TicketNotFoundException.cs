using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class TicketNotFoundException : BotExceptionBase
{
  public TicketNotFoundException() : base(Texts.TICKET_NOT_FOUND) { }
}