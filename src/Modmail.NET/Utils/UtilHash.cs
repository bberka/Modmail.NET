using System.Security.Cryptography;
using System.Text;

namespace Modmail.NET.Utils;

public static class UtilHash
{
  public static string CreateSha256Hash(string text) {
    var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
    return Convert.ToBase64String(hashedBytes);
  }

  public static string CreateSha256Hash(byte[] bytes) {
    var hashedBytes = SHA256.HashData(bytes);
    return Convert.ToBase64String(hashedBytes);
  }


  public static string CreateMd5Hash(string text) {
    var hashedBytes = MD5.HashData(Encoding.UTF8.GetBytes(text));
    return Convert.ToBase64String(hashedBytes);
  }

  public static string CreateMd5Hash(byte[] bytes) {
    var hashedBytes = MD5.HashData(bytes);
    return Convert.ToBase64String(hashedBytes);
  }
}