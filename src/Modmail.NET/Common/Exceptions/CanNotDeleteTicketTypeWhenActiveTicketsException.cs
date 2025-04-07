using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class CanNotDeleteTicketTypeWhenActiveTicketsException : ModmailBotException
{
  public CanNotDeleteTicketTypeWhenActiveTicketsException() : base(LangKeys.CanNotDeleteTicketTypeWhenActiveTickets.GetTranslation()) { }
}