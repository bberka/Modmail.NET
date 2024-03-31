namespace Modmail.NET.Exceptions;

public class ServerIsNotSetupException : BotExceptionBase
{
  public ServerIsNotSetupException() : base(LangData.This.GetTranslation(LangKeys.ROLE_NOT_FOUND_IN_TEAM)) { }
}