using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class NotFoundException : ModmailBotException
{
  public NotFoundException(LangKeys name) : base(LangProvider.This.GetTranslation(LangKeys.XNotFound, name)) {
    Name = name;
  }

  public LangKeys Name { get; }
}