namespace Modmail.NET.Exceptions;

public class UserAlreadyBlacklistedException : BotExceptionBase
{
  public UserAlreadyBlacklistedException() : base(LangData.This.GetTranslation(LangKeys.USER_ALREADY_BLACKLISTED)) { }
}