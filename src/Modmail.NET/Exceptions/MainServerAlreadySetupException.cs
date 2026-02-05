namespace Modmail.NET.Exceptions;

public class MainServerAlreadySetupException : BotExceptionBase
{
    public MainServerAlreadySetupException() : base(LangData.This.GetTranslation(LangKeys.MAIN_SERVER_ALREADY_SETUP))
    {
    }
}