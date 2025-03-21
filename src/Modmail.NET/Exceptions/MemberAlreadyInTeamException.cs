namespace Modmail.NET.Exceptions;

public class MemberAlreadyInTeamException : BotExceptionBase
{
  public MemberAlreadyInTeamException() : base(LangProvider.This.GetTranslation(LangKeys.MEMBER_ALREADY_IN_TEAM)) { }
}