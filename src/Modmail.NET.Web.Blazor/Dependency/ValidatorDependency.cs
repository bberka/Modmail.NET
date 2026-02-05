using FluentValidation;

namespace Modmail.NET.Web.Blazor.Dependency;

public static class ValidatorDependency
{
  public static void Configure(WebApplicationBuilder builder) {
    builder.Services.AddValidatorsFromAssemblyContaining(typeof(ModmailBotProjectMarker));
  }
}