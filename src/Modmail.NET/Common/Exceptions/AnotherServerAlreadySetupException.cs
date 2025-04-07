using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class AnotherServerAlreadySetupException : ModmailBotException
{
  public AnotherServerAlreadySetupException() : base(LangProvider.This.GetTranslation(LangKeys.AnotherServerAlreadySetup)) { }
}