using Modmail.NET;
using Modmail.NET.Utils;
using Modmail.NET.Web.Blazor.Components;
using Modmail.NET.Web.Blazor.Dependency;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

AspNetDependency.Configure(builder);
BlazorDependency.Configure(builder);
BusinessDependency.Configure(builder);
HangfireDependency.Configure(builder);
MediatorDependency.Configure(builder);
ValidatorDependency.Configure(builder);
AuthDependency.Configure(builder);

var app = builder.Build();

ServiceLocator.Initialize(app.Services);

#region DEV

if (!app.Environment.IsDevelopment()) {
  app.UseExceptionHandler("/Error", true);
  app.UseHsts();
}

#endregion


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();



//DO NOT CHANGE FOLLOWING METHODS ORDER
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
HangfireDependency.Initialize(app); //initializes UI 
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();



BusinessDependency.Initialize(app);
Log.Information("Starting Modmail.NET v{Version}", UtilVersion.GetReadableProductVersion());
app.Run();