namespace Modmail.NET.Exceptions;

public class UserNotFoundException : BotExceptionBase
{
  public UserNotFoundException(ulong id) : base(LangData.This.GetTranslation(LangKeys.USER_NOT_FOUND)) {
    Id = id;
  }

  public ulong Id { get; }
}