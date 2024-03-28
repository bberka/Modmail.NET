using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class MainServerAlreadySetupException : BotExceptionBase
{
  public MainServerAlreadySetupException() : base(Texts.MAIN_SERVER_ALREADY_SETUP) { }
}