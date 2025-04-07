using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class TeamNotExistsException : ModmailBotException
{
  public TeamNotExistsException() : base(LangProvider.This.GetTranslation(LangKeys.TeamNotExists)) { }
}