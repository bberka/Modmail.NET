using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class UserIsNotBlacklistedException : BotExceptionBase
{
  public UserIsNotBlacklistedException() : base(LangProvider.This.GetTranslation(LangKeys.UserIsNotBlacklisted)) { }
}