namespace Modmail.NET.Exceptions;

public sealed class NotFoundException : BotExceptionBase
{
  public NotFoundException(LangKeys name) : base(LangProvider.This.GetTranslation(LangKeys.X_NOT_FOUND, name)) {
    Name = name;
  }

  public LangKeys Name { get; }
}

public sealed class NotFoundWithException : BotExceptionBase
{
  public NotFoundWithException(LangKeys name, object id) : base(LangProvider.This.GetTranslation(LangKeys.X_NOT_FOUND, name, id)) {
    Name = name;
    Id = id;
  }

  public LangKeys Name { get; }
  public object Id { get; }
}

public sealed class NotFoundInException : BotExceptionBase
{
  public NotFoundInException(LangKeys name, LangKeys inName) : base(LangProvider.This.GetTranslation(LangKeys.X_NOT_FOUND_IN_Y, name, inName)) {
    Name = name;
    InName = inName;
  }

  public LangKeys Name { get; }
  public LangKeys InName { get; }
}