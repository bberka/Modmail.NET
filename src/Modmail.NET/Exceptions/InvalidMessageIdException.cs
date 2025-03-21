namespace Modmail.NET.Exceptions;

public class InvalidMessageIdException : BotExceptionBase
{
  public InvalidMessageIdException() : base(LangProvider.This.GetTranslation(LangKeys.INVALID_MESSAGE_ID)) { }
}