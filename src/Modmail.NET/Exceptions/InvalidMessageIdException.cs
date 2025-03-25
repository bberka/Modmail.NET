using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class InvalidMessageIdException : BotExceptionBase
{
  public InvalidMessageIdException() : base(LangProvider.This.GetTranslation(LangKeys.InvalidMessageId)) { }
}