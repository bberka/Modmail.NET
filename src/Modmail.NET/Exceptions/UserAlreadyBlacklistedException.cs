using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class UserAlreadyBlacklistedException : BotExceptionBase
{
  public UserAlreadyBlacklistedException() : base(Texts.USER_ALREADY_BLACKLISTED) { }
}