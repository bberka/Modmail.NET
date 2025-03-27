namespace Modmail.NET.Attributes;

public class PermissionCheckAttribute : Attribute
{
  public PermissionCheckAttribute(string policy) {
    Policy = policy;
  }

  public string Policy { get; }
}