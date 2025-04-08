using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class MainServerAlreadySetupException : ModmailBotException
{
  public MainServerAlreadySetupException() : base(LangProvider.This.GetTranslation(LangKeys.MainServerAlreadySetup)) { }
}