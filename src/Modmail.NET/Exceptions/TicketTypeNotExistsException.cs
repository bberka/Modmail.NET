namespace Modmail.NET.Exceptions;

public class TicketTypeNotExistsException : BotExceptionBase
{
  public TicketTypeNotExistsException(string name = null) : base(LangProvider.This.GetTranslation(LangKeys.TICKET_TYPE_NOT_EXISTS)) {
    Name = name;
  }

  public string Name { get; }
}