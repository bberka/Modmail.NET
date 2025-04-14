namespace Modmail.NET.Language;

public static class ExtLangData
{
  public static string Translate(this Lang key) {
    return LangProvider.This.GetTranslation(key);
  }

  public static string Translate(this Lang key, params string[] args) {
    return LangProvider.This.GetTranslation(key, args);
  }

  public static string Translate(this Lang key, params Lang[] args) {
    return LangProvider.This.GetTranslation(key, args.Select(x => x.ToString()).ToArray());
  }
}