namespace Modmail.NET.Exceptions;

public class NoTicketTypesFoundException : BotExceptionBase
{
  public NoTicketTypesFoundException() : base(LangData.This.GetTranslation(LangKeys.NO_TICKET_TYPES_FOUND)) { }
}