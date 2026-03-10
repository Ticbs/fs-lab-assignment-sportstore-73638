using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SportsStore.Models;
using SportsStore.Service.Payments;

var builder = WebApplication.CreateBuilder(args);

// ---- Serilog (lê do appsettings.json) ----
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// ---- MVC ----
builder.Services.AddControllersWithViews();

// ---- DB Contexts ----
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

// ---- Repositories ----
builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();

// ---- Cart (Session) ----
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));

// ---- Stripe settings + service ----
var stripeSettings =
    builder.Configuration.GetSection("Stripe").Get<StripeSettings>() ?? new StripeSettings();

builder.Services.AddSingleton(stripeSettings);
builder.Services.AddScoped<IPaymentService, StripePaymentService>();

var app = builder.Build();

// ---- Request logging ----
app.UseSerilogRequestLogging();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// session MUST be before routing
app.UseSession();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Log.Information("Tiago Borges 73638 - App started successfully");

app.Run();