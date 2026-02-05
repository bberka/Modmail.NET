namespace Modmail.NET.Exceptions;

public class CanNotDeleteTicketTypeWhenActiveTicketsException : BotExceptionBase
{
    public CanNotDeleteTicketTypeWhenActiveTicketsException() : base(LangKeys.CAN_NOT_DELETE_TICKET_TYPE_WHEN_ACTIVE_TICKETS.GetTranslation())
    {
    }
}