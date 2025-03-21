namespace Modmail.NET.Exceptions;

public class RoleAlreadyInTeamException : BotExceptionBase
{
  public RoleAlreadyInTeamException() : base(LangProvider.This.GetTranslation(LangKeys.ROLE_ALREADY_IN_TEAM)) { }
}