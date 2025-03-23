using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class NotFoundWithException : BotExceptionBase
{
  public NotFoundWithException(LangKeys name, object id) : base(LangProvider.This.GetTranslation(LangKeys.X_NOT_FOUND, name, id)) {
    Name = name;
    Id = id;
  }

  public LangKeys Name { get; }
  public object Id { get; }
}