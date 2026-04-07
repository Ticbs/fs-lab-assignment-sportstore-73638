using Serilog;
using SportsStore.PaymentService.Workers;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/payment.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHostedService<PaymentWorker>();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Payment Service started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Payment Service failed to start");
}
finally
{
    Log.CloseAndFlush();
}
