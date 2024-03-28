using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class TicketTypeAlreadyExistsException : BotExceptionBase
{
  public TicketTypeAlreadyExistsException(string name) : base(Texts.TICKET_TYPE_ALREADY_EXISTS) {
    Name = name;
  }

  public string Name { get; }
}