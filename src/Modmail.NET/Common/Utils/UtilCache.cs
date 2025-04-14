using System.Text;
using System.Text.Json;

namespace Modmail.NET.Common.Utils;

public static class UtilCache
{
  private static readonly string NullHashCode = "$__null_value".GetHashCode().ToString();

  public static string BuildCacheKeyFromT<T>(string mainKey, T? data = default) {
    var mainKeyStr = UtilHash.CreateSha256Hash(mainKey);
    var keyBuilder = new StringBuilder(mainKeyStr);

    if (data is not null) {
      string dataStr;
      if (data is string strData)
        dataStr = strData; // If it's already a string, use it directly
      else if (data is ValueType)
        dataStr = data.ToString() ?? NullHashCode; // For primitive types
      else
        dataStr = JsonSerializer.Serialize(data); // For complex objects

      // Hash the data string to keep it efficient and consistent
      var dataHash = UtilHash.CreateSha256Hash(dataStr);
      keyBuilder.Append(':').Append(dataHash);
    }

    return keyBuilder.ToString();
  }
}