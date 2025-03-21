using System.Text;
using Microsoft.Extensions.Options;
using Modmail.NET.Utils;
using Newtonsoft.Json;

namespace Modmail.NET.Language;

public sealed class LangProvider
{
  private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<LangKeys, string>> _languages;

  public LangProvider(IOptions<BotConfig> options) {
    var currentDir = Directory.GetCurrentDirectory();
    var langDir = Path.Combine(currentDir, "Language", "Data");
    if (!Directory.Exists(langDir))
      throw new DirectoryNotFoundException("Language data directory not found! Please make sure you have the correct directory structure. Expected directory : " + langDir);

    var files = Directory.GetFiles(langDir, "*.json");
    if (files.Length == 0) throw new FileNotFoundException("No language files found in the directory! Please make sure you have the correct directory structure. Expected directory : " + langDir);

    var dict = new Dictionary<string, IReadOnlyDictionary<LangKeys, string>>();
    foreach (var file in files) {
      var lang = Path.GetFileNameWithoutExtension(file);
      var data = File.ReadAllText(file);
      if (string.IsNullOrEmpty(data)) throw new JsonException("Failed to read language data for language : " + lang);

      var langData = JsonConvert.DeserializeObject<Dictionary<LangKeys, string>>(data);
      if (langData == null) throw new JsonException("Failed to deserialize language data for language : " + lang);

      var processLangData = new Dictionary<LangKeys, string>();
      foreach (var kp in langData) {
        var newValue = new StringBuilder(kp.Value)
                       .Replace("{NewLine}", Environment.NewLine)
                       .Replace("{Tab}", "\t")
                       .Replace("{BotPrefix}", options.Value.BotPrefix)
                       .Replace("{MainServerId}", options.Value.MainServerId.ToString())
                       .Replace("{Environment}", options.Value.Environment.ToString())
                       .Replace("{Version}", UtilVersion.GetVersion())
                       .ToString();
        processLangData.Add(kp.Key, newValue);
      }

      dict.Add(lang, processLangData);
    }

    var enumValues = Enum.GetValues<LangKeys>();
    //EXPORT ENUM VALUES TO JSON

    var enumValuesAsDictionary = enumValues.ToDictionary(x => x.ToString(), x => x.ToString());

    var enumValuesJson = JsonConvert.SerializeObject(enumValuesAsDictionary, Formatting.Indented);
    File.WriteAllText(Path.Combine(langDir, "_keys.json"), enumValuesJson);

    _languages = dict;
  }

  public static LangProvider This => ServiceLocator.GetLangProvider();


  private IReadOnlyDictionary<LangKeys, string> GetLanguage(string lang) {
    if (!_languages.TryGetValue(lang, out var language))
      throw new KeyNotFoundException("Language not found : " + lang);

    return language;
  }

  private string GetLanguage() {
    return ServiceLocator.GetBotConfig().DefaultLanguage;
  }

  public string GetTranslation(LangKeys key, params object[] args) {
    var lang = GetLanguage();
    if (lang.Contains('-')) lang = lang.Split('-')[0];

    var language = GetLanguage(lang);
    if (!language.TryGetValue(key, out var translation))
      return key.ToString();
    // throw new KeyNotFoundException("Translation not found for key : " + key);

    //try parse args to enum LangKeys and if exists replace with translation

    if (args.Length == 0) return translation;
    var newArgs = new List<object>();
    foreach (var arg in args)
      newArgs.Add(Enum.TryParse<LangKeys>(arg.ToString(), out var newArg)
                    ? GetTranslation(newArg)
                    : arg);

    return string.Format(translation, newArgs);
  }
}