namespace Modmail.NET.Static;

public enum TeamPermissionLevel : byte
{
  Support,
  Moderator = 100,
  Admin = 200,
  Owner = 255
}