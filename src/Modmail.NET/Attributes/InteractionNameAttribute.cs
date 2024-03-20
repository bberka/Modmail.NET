namespace Modmail.NET.Attributes;

public class InteractionNameAttribute : Attribute
{
  public string Name { get; }
  public int ParamCount { get; }

  public InteractionNameAttribute(string name, int paramCount) {
    Name = name;
    ParamCount = paramCount;
  }
}