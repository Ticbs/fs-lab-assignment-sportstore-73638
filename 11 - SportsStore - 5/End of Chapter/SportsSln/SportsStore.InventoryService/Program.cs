using Serilog;
using SportsStore.InventoryService.Workers;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/inventory.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHostedService<InventoryWorker>();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Inventory Service started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Inventory Service failed to start");
}
finally
{
    Log.CloseAndFlush();
}
