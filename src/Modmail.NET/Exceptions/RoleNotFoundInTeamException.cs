namespace Modmail.NET.Exceptions;

public class RoleNotFoundInTeamException : BotExceptionBase
{
  public RoleNotFoundInTeamException() : base(LangData.This.GetTranslation(LangKeys.ROLE_NOT_FOUND_IN_TEAM)) { }
}