using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class NotJoinedMainServerException : ModmailBotException
{
  public NotJoinedMainServerException() : base(LangProvider.This.GetTranslation(LangKeys.NotJoinedMainServer)) { }
}