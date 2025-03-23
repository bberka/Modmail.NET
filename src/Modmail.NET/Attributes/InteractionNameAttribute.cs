namespace Modmail.NET.Attributes;

public class InteractionNameAttribute : Attribute
{
  public InteractionNameAttribute(string name, int paramCount) {
    Name = name;
    ParamCount = paramCount;
  }

  public string Name { get; }
  public int ParamCount { get; }
}