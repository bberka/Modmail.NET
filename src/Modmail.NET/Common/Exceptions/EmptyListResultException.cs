using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class EmptyListResultException : ModmailBotException
{
  public EmptyListResultException(LangKeys name) : base(LangKeys.NoXFound.GetTranslation(name)) {
    Name = name;
  }

  public LangKeys Name { get; }
}