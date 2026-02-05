using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class InvalidUserIdException : ModmailBotException
{
  public InvalidUserIdException() : base(LangProvider.This.GetTranslation(LangKeys.InvalidUser)) { }
}