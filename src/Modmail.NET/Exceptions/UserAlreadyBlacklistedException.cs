using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class UserAlreadyBlacklistedException : BotExceptionBase
{
  public UserAlreadyBlacklistedException() : base(LangProvider.This.GetTranslation(LangKeys.UserAlreadyBlacklisted)) { }
}