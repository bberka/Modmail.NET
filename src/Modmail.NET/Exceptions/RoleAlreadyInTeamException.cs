using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class RoleAlreadyInTeamException : BotExceptionBase
{
  public RoleAlreadyInTeamException() : base(LangProvider.This.GetTranslation(LangKeys.RoleAlreadyInTeam)) { }
}