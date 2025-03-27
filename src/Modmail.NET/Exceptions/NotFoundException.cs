using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class NotFoundException : BotExceptionBase
{
  public NotFoundException(LangKeys name) : base(LangProvider.This.GetTranslation(LangKeys.XNotFound, name)) {
    Name = name;
  }

  public LangKeys Name { get; }
}