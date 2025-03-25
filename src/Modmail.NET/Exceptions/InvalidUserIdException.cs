using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class InvalidUserIdException : BotExceptionBase
{
  public InvalidUserIdException() : base(LangProvider.This.GetTranslation(LangKeys.InvalidUser)) { }
}