using Serilog;
using SportsStore.CustomerPortal;
using SportsStore.CustomerPortal.Components;
using SportsStore.CustomerPortal.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/portal.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.AddHttpClient<OrderApiService>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5049/");
    });

    var app = builder.Build();

    app.UseStaticFiles();
    app.UseAntiforgery();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    Log.Information("Customer Portal started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Customer Portal failed to start");
}
finally
{
    Log.CloseAndFlush();
}
