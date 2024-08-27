namespace Modmail.NET.Exceptions;

public class InvalidMessageIdException : BotExceptionBase
{
  public InvalidMessageIdException() : base(LangData.This.GetTranslation(LangKeys.INVALID_MESSAGE_ID)) { }
}