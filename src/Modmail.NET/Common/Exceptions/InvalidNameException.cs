using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class InvalidNameException : ModmailBotException
{
  public InvalidNameException(string name) : base(LangProvider.This.GetTranslation(LangKeys.InvalidName)) {
    Name = name;
  }

  public string Name { get; }
}