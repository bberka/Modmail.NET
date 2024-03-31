namespace Modmail.NET.Exceptions;

public class NoBlacklistedUsersFoundException : BotExceptionBase
{
  public NoBlacklistedUsersFoundException() : base(LangData.This.GetTranslation(LangKeys.NO_BLACKLISTED_USERS_FOUND)) { }
}