using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class InvalidUserIdException : BotExceptionBase
{
  public InvalidUserIdException() : base(Texts.INVALID_USER) { }
}