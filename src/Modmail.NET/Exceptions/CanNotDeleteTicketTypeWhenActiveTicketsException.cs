using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class CanNotDeleteTicketTypeWhenActiveTicketsException : BotExceptionBase
{
  public CanNotDeleteTicketTypeWhenActiveTicketsException() : base(LangKeys.CanNotDeleteTicketTypeWhenActiveTickets.GetTranslation()) { }
}