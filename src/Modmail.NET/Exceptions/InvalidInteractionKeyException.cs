using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class InvalidInteractionKeyException : BotExceptionBase
{
  public InvalidInteractionKeyException() : base(LangProvider.This.GetTranslation(LangKeys.InvalidInteractionKey)) { }
}