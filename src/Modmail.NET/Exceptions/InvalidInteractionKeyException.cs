using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class InvalidInteractionKeyException : BotExceptionBase
{
  public InvalidInteractionKeyException() : base(Texts.INVALID_INTERACTION_KEY) { }
}