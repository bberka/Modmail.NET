using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class MemberAlreadyInTeamException : BotExceptionBase
{
  public MemberAlreadyInTeamException() : base(Texts.MEMBER_ALREADY_IN_TEAM) { }
}