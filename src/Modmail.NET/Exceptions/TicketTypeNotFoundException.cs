using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class TicketTypeNotFoundException : BotExceptionBase
{
  public TicketTypeNotFoundException(string keyOrName) : base(Texts.TICKET_TYPE_NOT_FOUND, string.Format(Texts.TICKET_TYPE_NOT_FOUND_DESCRIPTION, keyOrName)) {
    KeyOrName = keyOrName;
  }

  public TicketTypeNotFoundException() : base(Texts.TICKET_TYPE_NOT_FOUND) {
    KeyOrName = string.Empty;
  }

  public string KeyOrName { get; }
}