using System.Diagnostics;
using AspectInjector.Broker;
using Serilog;

namespace Modmail.NET.Common.Aspects;

/// <summary>
///     Performance Logger Aspect for logging long running actions, with a default threshold of 1000 ms. Desired threshold
///     can be set in constructor.
/// </summary>
[Aspect(Scope.PerInstance)]
[Injection(typeof(PerformanceLoggerAspect))]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PerformanceLoggerAspect : Attribute
{
    [Advice(Kind.Around)]
    public object Intercept(
        [Argument(Source.Target)] Func<object[], object> target,
        [Argument(Source.Arguments)] object[] args,
        [Argument(Source.Name)] string methodName,
        [Argument(Source.Type)] Type type,
        [Argument(Source.ReturnType)] Type returnType
    )
    {
        var className = type.Name;
        var argList = string.Join(",", args);
        var sw = Stopwatch.StartNew();
        try
        {
            var result = target(args);
            return result;
        }
        finally
        {
            sw.Stop();
            if (1000 < sw.ElapsedMilliseconds)
                Log.Warning("{ClassName}.{MethodName}({ArgList}) Action too long to execute, executed in {ElapsedMilliseconds} ms", className,
                    methodName, argList, sw.ElapsedMilliseconds);
        }
    }
}