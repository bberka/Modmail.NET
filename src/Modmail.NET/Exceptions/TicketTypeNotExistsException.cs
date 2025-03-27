using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class TicketTypeNotExistsException : BotExceptionBase
{
  public TicketTypeNotExistsException(string name = null) : base(LangProvider.This.GetTranslation(LangKeys.TicketTypeNotExists)) {
    Name = name;
  }

  public string Name { get; }
}