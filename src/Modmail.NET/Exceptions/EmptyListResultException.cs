using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class EmptyListResultException : BotExceptionBase
{
  public EmptyListResultException(LangKeys name) : base(LangKeys.NoXFound.GetTranslation(name)) {
    Name = name;
  }

  public LangKeys Name { get; }
}