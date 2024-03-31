namespace Modmail.NET.Exceptions;

public class TicketTypeNotFoundException : BotExceptionBase
{
  public TicketTypeNotFoundException(string keyOrName) : base(LangData.This.GetTranslation(LangKeys.TICKET_TYPE_NOT_FOUND), LangData.This.GetTranslation(LangKeys.TICKET_TYPE_NOT_FOUND, keyOrName)) {
    KeyOrName = keyOrName;
  }

  public TicketTypeNotFoundException() : base(LangData.This.GetTranslation(LangKeys.TICKET_TYPE_NOT_FOUND)) {
    KeyOrName = string.Empty;
  }

  public string KeyOrName { get; }
}