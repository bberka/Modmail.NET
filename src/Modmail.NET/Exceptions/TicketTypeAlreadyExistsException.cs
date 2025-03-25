using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class TicketTypeAlreadyExistsException : BotExceptionBase
{
  public TicketTypeAlreadyExistsException(string name) : base(LangProvider.This.GetTranslation(LangKeys.TicketTypeAlreadyExists)) {
    Name = name;
  }

  public string Name { get; }
}