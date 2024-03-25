using AspectInjector.Broker;
using Microsoft.Extensions.Caching.Memory;

namespace Modmail.NET.Aspects;

/// <summary>
///   Basic caching aspect for caching method results.
///   Cache key is built from namespace, class name, method name and arguments.
///   It uses EasMemoryCache singleton class.
/// </summary>
[Aspect(Scope.PerInstance)]
[Injection(typeof(CacheAspect))]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CacheAspect : Attribute
{
  public CacheAspect() { }

  /// <summary>
  ///  Cache duration in seconds. Default is 60 seconds.
  /// </summary>
  public int CacheSeconds { get; set; } = 60;

  public bool DoNotCacheIfNull { get; set; } = true;

  [Advice(Kind.Around)]
  public object Intercept(
    [Argument(Source.Target)] Func<object[], object> target,
    [Argument(Source.Arguments)] object[] args,
    [Argument(Source.Name)] string methodName,
    [Argument(Source.Type)] Type type,
    [Argument(Source.ReturnType)] Type returnType) {
    var cacheKey = BuildCacheKey(type, methodName, args);
    var memoryCache = ServiceLocator.Get<IMemoryCache>();
    var cacheResult = memoryCache.Get(cacheKey);
    if (cacheResult is not null) return cacheResult;
    var result = target(args);
    if (result is null && DoNotCacheIfNull) return result;
    memoryCache.Set(cacheKey, result, TimeSpan.FromSeconds(CacheSeconds));
    return result;
  }

  private static string BuildCacheKey(Type classType, string methodName, object[] args) {
    var nameSpace = classType.Namespace;
    var classTypeName = classType.Name;
    var cacheKey = $"{nameSpace}.{classTypeName}.{methodName}";
    if (args.Length == 0) return cacheKey;
    var array = args.Select(x => x.GetHashCode()).ToList();
    var joined = string.Join(", ", array);
    cacheKey += $".{joined}";
    return cacheKey;
  }
}