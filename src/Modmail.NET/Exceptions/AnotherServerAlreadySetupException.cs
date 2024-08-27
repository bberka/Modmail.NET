namespace Modmail.NET.Exceptions;

public class AnotherServerAlreadySetupException : BotExceptionBase
{
  public AnotherServerAlreadySetupException() : base(LangData.This.GetTranslation(LangKeys.ANOTHER_SERVER_ALREADY_SETUP)) { }
}