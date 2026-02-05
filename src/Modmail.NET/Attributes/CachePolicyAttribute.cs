namespace Modmail.NET.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CachePolicyAttribute : Attribute
{
  public CachePolicyAttribute(string cacheKey, int durationInSeconds, bool includeRequestDataToCacheKey = true) {
    CacheKey = cacheKey;
    DurationInSeconds = durationInSeconds;
    IncludeRequestDataToCacheKey = includeRequestDataToCacheKey;
  }

  public string CacheKey { get; }
  public int DurationInSeconds { get; }
  public bool IncludeRequestDataToCacheKey { get; }
}