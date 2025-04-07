using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class UserAlreadyBlacklistedException : ModmailBotException
{
  public UserAlreadyBlacklistedException() : base(LangProvider.This.GetTranslation(LangKeys.UserAlreadyBlacklisted)) { }
}