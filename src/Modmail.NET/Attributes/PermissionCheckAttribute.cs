namespace Modmail.NET.Attributes;

public sealed class PermissionCheckAttribute : Attribute
{
  private readonly string _policy;

  public PermissionCheckAttribute(string policy) {
    _policy = policy;
  }
}