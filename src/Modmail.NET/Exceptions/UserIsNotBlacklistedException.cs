using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class UserIsNotBlacklistedException : BotExceptionBase
{
  public UserIsNotBlacklistedException() : base(Texts.USER_IS_NOT_BLACKLISTED) { }
}