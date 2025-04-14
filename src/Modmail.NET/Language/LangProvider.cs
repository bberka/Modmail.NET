using System.Text;
using Microsoft.Extensions.Options;
using Modmail.NET.Common.Utils;
using Newtonsoft.Json;
using Serilog;

namespace Modmail.NET.Language;

public class LangProvider
{
  private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<Lang, string>> _languages;

  public LangProvider(IOptions<BotConfig> options) {
    var currentDir = Directory.GetCurrentDirectory();
    var langDir = Path.Combine(currentDir, "wwwroot", "resources");
    if (!Directory.Exists(langDir))
      throw new DirectoryNotFoundException("Language data directory not found! Please make sure you have the correct directory structure. Expected directory : " + langDir);

    var files = Directory.GetFiles(langDir, "*.json");
    if (files.Length == 0) throw new FileNotFoundException("No language files found in the directory! Please make sure you have the correct directory structure. Expected directory : " + langDir);

    var dict = new Dictionary<string, IReadOnlyDictionary<Lang, string>>();
    foreach (var file in files) {
      var lang = Path.GetFileNameWithoutExtension(file);
      var data = File.ReadAllText(file);
      if (string.IsNullOrEmpty(data)) throw new JsonException("Failed to read language data for language : " + lang);

      var langData = JsonConvert.DeserializeObject<Dictionary<Lang, string>>(data);
      if (langData == null) throw new JsonException("Failed to deserialize language data for language : " + lang);

      var processLangData = new Dictionary<Lang, string>();
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

    _languages = dict;
    Log.Information("Language data loaded successfully {SupportedLanguages}", string.Join(", ", dict.Keys));
  }

  public static LangProvider This => ServiceLocator.GetLangProvider();


  private IReadOnlyDictionary<Lang, string> GetLanguage(string lang) {
    if (!_languages.TryGetValue(lang, out var language))
      throw new KeyNotFoundException("Language not found : " + lang);

    return language;
  }

  private string GetLanguage() {
    return ServiceLocator.GetBotConfig().DefaultLanguage;
  }

  public string GetTranslation(Lang key, params string[] args) {
    var lang = GetLanguage();
    if (lang.Contains('-')) lang = lang.Split('-')[0];

    var language = GetLanguage(lang);
    if (!language.TryGetValue(key, out var translation))
      return key.ToString();
    // throw new KeyNotFoundException("Translation not found for key : " + key);

    //try parse args to enum Lang and if exists replace with translation
    if (args.Length == 0) return translation;
    var newArgs = new List<string>();
    foreach (var arg in args)
      newArgs.Add(Enum.TryParse<Lang>(arg, out var newArg)
                    ? GetTranslation(newArg)
                    : arg);

    var text = string.Format(translation, [..newArgs]);
    return text;
  }
}