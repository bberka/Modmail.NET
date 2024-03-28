using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class AnotherServerAlreadySetupException : BotExceptionBase
{
  public AnotherServerAlreadySetupException() : base(Texts.ANOTHER_SERVER_ALREADY_SETUP) { }
}