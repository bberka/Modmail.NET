using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class TeamAlreadyExistsException : ModmailBotException
{
  public TeamAlreadyExistsException() : base(LangProvider.This.GetTranslation(LangKeys.TeamAlreadyExists)) { }
}