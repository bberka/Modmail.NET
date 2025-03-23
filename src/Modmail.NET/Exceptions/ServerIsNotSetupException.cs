using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class ServerIsNotSetupException : BotExceptionBase
{
  public ServerIsNotSetupException() : base(LangProvider.This.GetTranslation(LangKeys.ROLE_NOT_FOUND_IN_TEAM)) { }
}