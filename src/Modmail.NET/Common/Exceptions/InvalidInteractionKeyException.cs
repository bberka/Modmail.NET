using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class InvalidInteractionKeyException : ModmailBotException
{
  public InvalidInteractionKeyException() : base(LangProvider.This.GetTranslation(LangKeys.InvalidInteractionKey)) { }
}