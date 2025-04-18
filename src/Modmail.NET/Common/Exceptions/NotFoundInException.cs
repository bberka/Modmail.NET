using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class NotFoundInException : ModmailBotException
{
  public NotFoundInException(LangKeys name, LangKeys inName) : base(LangProvider.This.GetTranslation(LangKeys.XNotFoundInY, name, inName)) {
    Name = name;
    InName = inName;
  }

  public LangKeys Name { get; }
  public LangKeys InName { get; }
}