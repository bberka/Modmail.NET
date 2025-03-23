using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class MainServerAlreadySetupException : BotExceptionBase
{
  public MainServerAlreadySetupException() : base(LangProvider.This.GetTranslation(LangKeys.MAIN_SERVER_ALREADY_SETUP)) { }
}