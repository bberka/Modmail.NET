﻿using System.Reflection;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Utils;
using Serilog;

namespace Modmail.NET.Pipeline;

public class CachingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
  private readonly IMemoryCache _cache;

  public CachingPipelineBehavior(IMemoryCache cache) {
    _cache = cache;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
    var cacheAttribute = typeof(TRequest).GetCustomAttribute<CachePolicyAttribute>();
    if (cacheAttribute == null) return await next();
    var cacheKey = UtilCache.BuildCacheKeyFromT(cacheAttribute.CacheKey, cacheAttribute.IncludeRequestDataToCacheKey
                                                                           ? request
                                                                           : default);
    if (_cache.TryGetValue(cacheKey, out TResponse cachedResponse) && cachedResponse is not null) {
      Log.Verbose("Returning cached data for: {CacheKey}", cacheKey);
      return cachedResponse;
    }

    var response = await next();
    _cache.Set(cacheKey, response, TimeSpan.FromSeconds(cacheAttribute.DurationInSeconds));
    Log.Verbose("Caching response for: {CacheKey} (Duration: {CacheAttributeDurationInSeconds}s)", cacheKey, cacheAttribute.DurationInSeconds);
    return response;
  }
}