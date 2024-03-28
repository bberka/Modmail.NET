using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class RoleNotFoundInTeamException : BotExceptionBase
{
  public RoleNotFoundInTeamException() : base(Texts.ROLE_NOT_FOUND_IN_TEAM) { }
}