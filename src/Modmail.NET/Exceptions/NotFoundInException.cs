using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class NotFoundInException : BotExceptionBase
{
  public NotFoundInException(LangKeys name, LangKeys inName) : base(LangProvider.This.GetTranslation(LangKeys.XNotFoundInY, name, inName)) {
    Name = name;
    InName = inName;
  }

  public LangKeys Name { get; }
  public LangKeys InName { get; }
}