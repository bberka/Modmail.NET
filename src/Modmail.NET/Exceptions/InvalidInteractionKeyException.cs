namespace Modmail.NET.Exceptions;

public class InvalidInteractionKeyException : BotExceptionBase
{
    public InvalidInteractionKeyException() : base(LangData.This.GetTranslation(LangKeys.INVALID_INTERACTION_KEY))
    {
    }
}