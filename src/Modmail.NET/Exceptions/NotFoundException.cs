namespace Modmail.NET.Exceptions;

public sealed class NotFoundException : BotExceptionBase
{
  public NotFoundException(LangKeys name) : base(LangProvider.This.GetTranslation(LangKeys.X_NOT_FOUND, name)) {
    Name = name;
  }

  public LangKeys Name { get; }
}