using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class UserIsNotBlacklistedException : ModmailBotException
{
  public UserIsNotBlacklistedException() : base(LangProvider.This.GetTranslation(LangKeys.UserIsNotBlacklisted)) { }
}