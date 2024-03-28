using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class NoTicketTypesFoundException : BotExceptionBase
{
  public NoTicketTypesFoundException() : base(Texts.NO_TICKET_TYPES_FOUND) { }
}