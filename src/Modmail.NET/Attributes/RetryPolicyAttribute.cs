namespace Modmail.NET.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RetryPolicyAttribute : Attribute
{
    public RetryPolicyAttribute(int retryCount = 3, int delaySeconds = 2)
    {
        RetryCount = retryCount;
        DelaySeconds = delaySeconds;
    }

    public int RetryCount { get; }
    public int DelaySeconds { get; }
}