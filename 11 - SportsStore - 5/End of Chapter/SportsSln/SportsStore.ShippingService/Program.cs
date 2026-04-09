using Serilog;
using SportsStore.ShippingService.Workers;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/shipping.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHostedService<ShippingWorker>();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Shipping Service started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Shipping Service failed to start");
}
finally
{
    Log.CloseAndFlush();
}
