namespace Modmail.NET.Exceptions;

public sealed class EmptyListResultException : BotExceptionBase
{
  public EmptyListResultException(LangKeys name) : base(LangKeys.NO_X_FOUND.GetTranslation(name)) {
    Name = name;
  }

  public LangKeys Name { get; }
}