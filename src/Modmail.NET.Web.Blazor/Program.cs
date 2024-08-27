using Modmail.NET.Database;
using Modmail.NET.Web.Blazor.Components;
using Modmail.NET.Web.Blazor.Services;
using Modmail.NET.Web.Shared;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();

builder.Services.AddHostedService<ModmailHostedService>();
builder.Services.AddDbContext<ModmailDbContext>();

builder.Services.AddRadzenCookieThemeService(options => {
  options.Name = WebSharedConstants.THEME_COOKIE_NAME; // The name of the cookie
  options.Duration = TimeSpan.FromDays(365); // The duration of the cookie
});

builder.Services.AddRadzenComponents();


var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
  app.UseExceptionHandler("/Error", true);
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();