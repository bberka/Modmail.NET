using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class MemberNotFoundInTeamException : BotExceptionBase
{
  public MemberNotFoundInTeamException() : base(Texts.MEMBER_NOT_FOUND_IN_TEAM) { }
}