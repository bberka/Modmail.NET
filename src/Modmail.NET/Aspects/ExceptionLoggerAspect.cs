using AspectInjector.Broker;
using Serilog;

namespace Modmail.NET.Aspects;

/// <summary>
///   Basic exception logger aspect for logging exceptions and rethrowing them with a more descriptive message.
/// </summary>
[Aspect(Scope.PerInstance)]
[Injection(typeof(ExceptionLoggerAspect))]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ExceptionLoggerAspect : Attribute
{
  [Advice(Kind.Around)]
  public object Intercept(
    [Argument(Source.Target)] Func<object[], object> target,
    [Argument(Source.Arguments)] object[] args,
    [Argument(Source.Name)] string methodName,
    [Argument(Source.Type)] Type type,
    [Argument(Source.ReturnType)] Type returnType) {
    var className = type.Name;
    var argList = string.Join(",", args);
    try {
      return target(args);
    }
    catch (Exception ex) {
      var msg = $"ExceptionInfo in {className}.{methodName}({argList}) Key:{ex.Message}";
      Log.Fatal(ex, msg);
      throw new Exception(msg, ex);
    }
  }
}