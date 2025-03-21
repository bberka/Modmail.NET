using MediatR;
using Serilog;

namespace Modmail.NET.Pipeline;

public sealed class LoggerPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
    var reqName = typeof(TRequest).Name;
    try {
      var response = await next();
      Log.Verbose("[MEDIATR] [{ReqName}] RUN FINISHED", reqName);
      return response;
    }
    catch (Exception ex) {
      Log.Error(ex, "[MEDIATR] [{ReqName}] EXCEPTION", reqName);
      throw;
    }
  }
}