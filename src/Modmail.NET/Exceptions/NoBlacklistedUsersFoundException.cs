using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class NoBlacklistedUsersFoundException : BotExceptionBase
{
  public NoBlacklistedUsersFoundException() : base(Texts.NO_BLACKLISTED_USERS_FOUND) { }
}