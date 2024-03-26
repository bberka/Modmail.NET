using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class UserNotFoundException : BotExceptionBase
{
  public UserNotFoundException(ulong id) : base(Texts.USER_NOT_FOUND) {
    Id = id;
  }

  public ulong Id { get; }
}