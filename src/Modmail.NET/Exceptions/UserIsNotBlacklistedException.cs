namespace Modmail.NET.Exceptions;

public class UserIsNotBlacklistedException : BotExceptionBase
{
  public UserIsNotBlacklistedException() : base(LangProvider.This.GetTranslation(LangKeys.USER_IS_NOT_BLACKLISTED)) { }
}