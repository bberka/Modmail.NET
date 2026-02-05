using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class TicketAlreadyClosedException : ModmailBotException
{
  public TicketAlreadyClosedException() : base(LangProvider.This.GetTranslation(LangKeys.TicketAlreadyClosed)) { }
}