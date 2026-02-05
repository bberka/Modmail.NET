namespace Modmail.NET.Exceptions;

public class UserIsNotBlacklistedException : BotExceptionBase
{
    public UserIsNotBlacklistedException() : base(LangData.This.GetTranslation(LangKeys.USER_IS_NOT_BLACKLISTED))
    {
    }
}