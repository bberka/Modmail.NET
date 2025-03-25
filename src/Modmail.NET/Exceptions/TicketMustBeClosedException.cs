using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class TicketMustBeClosedException : BotExceptionBase
{
  public TicketMustBeClosedException() : base(LangProvider.This.GetTranslation(LangKeys.TicketMustBeClosed)) { }
}