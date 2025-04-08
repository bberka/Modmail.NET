using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class NotFoundWithException : ModmailBotException
{
  public NotFoundWithException(LangKeys name, object id) : base(LangProvider.This.GetTranslation(LangKeys.XNotFound, name, id)) {
    Name = name;
    Id = id;
  }

  public LangKeys Name { get; }
  public object Id { get; }
}