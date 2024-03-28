using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class InvalidNameException : BotExceptionBase
{
  public InvalidNameException(string name) : base(Texts.INVALID_NAME) {
    Name = name;
  }

  public string Name { get; }
}