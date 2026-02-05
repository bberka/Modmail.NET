namespace Modmail.NET.Exceptions;

public class RoleAlreadyInTeamException : BotExceptionBase
{
    public RoleAlreadyInTeamException() : base(LangData.This.GetTranslation(LangKeys.ROLE_ALREADY_IN_TEAM))
    {
    }
}