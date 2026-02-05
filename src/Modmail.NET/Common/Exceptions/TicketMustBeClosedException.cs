using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class TicketMustBeClosedException : ModmailBotException
{
  public TicketMustBeClosedException() : base(LangProvider.This.GetTranslation(LangKeys.TicketMustBeClosed)) { }
}