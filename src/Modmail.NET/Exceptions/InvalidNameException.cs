using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class InvalidNameException : BotExceptionBase
{
  public InvalidNameException(string name) : base(LangProvider.This.GetTranslation(LangKeys.InvalidName)) {
    Name = name;
  }

  public string Name { get; }
}