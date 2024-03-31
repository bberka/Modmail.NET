namespace Modmail.NET.Exceptions;

public class InvalidUserIdException : BotExceptionBase
{
  public InvalidUserIdException() : base(LangData.This.GetTranslation(LangKeys.INVALID_USER)) { }
}