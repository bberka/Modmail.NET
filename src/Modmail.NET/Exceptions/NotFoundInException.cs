namespace Modmail.NET.Exceptions;

public sealed class NotFoundInException : BotExceptionBase
{
  public NotFoundInException(LangKeys name, LangKeys inName) : base(LangProvider.This.GetTranslation(LangKeys.X_NOT_FOUND_IN_Y, name, inName)) {
    Name = name;
    InName = inName;
  }

  public LangKeys Name { get; }
  public LangKeys InName { get; }
}