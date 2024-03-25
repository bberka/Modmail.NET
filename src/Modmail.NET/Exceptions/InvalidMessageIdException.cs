using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class InvalidMessageIdException : BotExceptionBase
{
  public InvalidMessageIdException() : base(Texts.INVALID_MESSAGE_ID) { }
}