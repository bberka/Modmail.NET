using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class NotFoundWithException : BotExceptionBase
{
  public NotFoundWithException(LangKeys name, object id) : base(LangProvider.This.GetTranslation(LangKeys.XNotFound, name, id)) {
    Name = name;
    Id = id;
  }

  public LangKeys Name { get; }
  public object Id { get; }
}