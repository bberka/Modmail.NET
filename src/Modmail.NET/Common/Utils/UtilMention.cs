using System.Text;
using Modmail.NET.Features.Permission.Models;

namespace Modmail.NET.Common.Utils;

public static class UtilMention
{
  public static string GetMentionsMessageString(IEnumerable<PermissionInfo> permissions) {
    var sb = new StringBuilder();
    foreach (var perm in permissions) sb.AppendLine(perm.GetMention());

    return sb.ToString();
  }
}