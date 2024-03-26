using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class RoleAlreadyInTeamException : BotExceptionBase
{
  public RoleAlreadyInTeamException() : base(Texts.ROLE_ALREADY_IN_TEAM) { }
}