namespace Modmail.NET.Exceptions;

public class MemberAlreadyInTeamException : BotExceptionBase
{
    public MemberAlreadyInTeamException() : base(LangData.This.GetTranslation(LangKeys.MEMBER_ALREADY_IN_TEAM))
    {
    }
}