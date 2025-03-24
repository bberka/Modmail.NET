namespace Modmail.NET.Attributes;

public class PermissionCheckAttribute : Attribute
{
  public string Policy { get; }


  public PermissionCheckAttribute(string policy) {
    Policy = policy;
  }
}