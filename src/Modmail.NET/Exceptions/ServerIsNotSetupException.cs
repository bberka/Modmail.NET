using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class ServerIsNotSetupException : BotExceptionBase
{
  public ServerIsNotSetupException() : base(Texts.SERVER_NOT_SETUP) { }
}