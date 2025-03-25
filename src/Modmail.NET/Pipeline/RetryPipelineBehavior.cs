using System.Reflection;
using MediatR;
using Modmail.NET.Attributes;
using Polly;
using Serilog;

namespace Modmail.NET.Pipeline;

public class RetryPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
    var retryAttribute = typeof(TRequest).GetCustomAttribute<RetryPolicyAttribute>();
    if (retryAttribute == null) return await next();

    var retryPolicy = Policy
                      .Handle<Exception>()
                      .WaitAndRetryAsync(
                                         retryAttribute.RetryCount,
                                         retryAttempt => TimeSpan.FromSeconds(retryAttribute.DelaySeconds * retryAttempt), // Exponential backoff
                                         (exception, timeSpan, retryCount, _) => {
                                           Log.Warning("Retry {RetryCount}/{RetryAttributeRetryCount} after {TimeSpanTotalSeconds} seconds due to: {ExceptionMessage}",
                                                       retryCount,
                                                       retryAttribute.RetryCount,
                                                       timeSpan.TotalSeconds,
                                                       exception.Message);
                                         });

    return await retryPolicy.ExecuteAsync(() => next());
  }
}