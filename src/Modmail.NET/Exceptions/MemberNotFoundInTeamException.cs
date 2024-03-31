namespace Modmail.NET.Exceptions;

public class MemberNotFoundInTeamException : BotExceptionBase
{
  public MemberNotFoundInTeamException() : base(LangData.This.GetTranslation(LangKeys.MEMBER_NOT_FOUND_IN_TEAM)) { }
}