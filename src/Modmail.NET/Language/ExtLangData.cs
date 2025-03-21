namespace Modmail.NET.Language;

public static class ExtLangData
{
  public static string GetTranslation(this LangKeys key) {
    return LangProvider.This.GetTranslation(key);
  }

  // public static string GetTranslation(this LangKeys key, string lang) => LangData.This.GetTranslation(lang, key);
  public static string GetTranslation(this LangKeys key, params object[] args) {
    return LangProvider.This.GetTranslation(key, args);
  }
}