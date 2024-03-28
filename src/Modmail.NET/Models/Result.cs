namespace Modmail.NET.Models;

public record Result(bool Success, string TitleMessage, string Message = "")
{
  public static Result Ok(string titleMessage, string message = "", object? value = null) => new(true, titleMessage, message);

  public static Result Failure(string titleMessage, string message = "", object? value = null) => new(false, titleMessage, message);
}

public record Result<T>(bool Success, string TitleMessage, string Message = "", T? Value = default)
{
  public static Result<T> Ok(T value, string titleMessage, string message = "") => new(true, titleMessage, message, value);

  public static Result<T> Failure(string titleMessage, string message = "", T? value = default) => new(false, titleMessage, message, value);
}