using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class FeedbackAlreadySubmittedException : ModmailBotException
{
  public FeedbackAlreadySubmittedException() : base(LangProvider.This.GetTranslation(LangKeys.FeedbackAlreadySubmitted)) { }
}