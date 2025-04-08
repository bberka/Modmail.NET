using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class InvalidMessageIdException : ModmailBotException
{
  public InvalidMessageIdException() : base(LangProvider.This.GetTranslation(LangKeys.InvalidMessageId)) { }
}