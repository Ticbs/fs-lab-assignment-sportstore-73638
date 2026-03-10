using Microsoft.AspNetCore.Mvc;
using Serilog;
using SportsStore.Service.Payments;

namespace SportsStore.Controllers;

public class PaymentController : Controller
{
    private readonly IPaymentService _payments;

    public PaymentController(IPaymentService payments)
    {
        _payments = payments;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(string orderId, decimal amount, string currency = "eur")
    {
        try
        {
            Log.Information("Tiago Borges 73638 - Checkout start {OrderId} {Amount} {Currency}", orderId, amount, currency);

            var result = await _payments.CreateCheckoutSessionAsync(amount, currency, orderId);

            Log.Information("Tiago Borges 73638 - Checkout session created {OrderId} {SessionId}", orderId, result.SessionId);

            return Redirect(result.Url);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Tiago Borges 73638 - Stripe checkout failed {OrderId}", orderId);
            return RedirectToAction("Cancel", new { orderId });
        }
    }

    [HttpGet]
    public IActionResult Success(string orderId, string session_id)
    {
        Log.Information("Tiago Borges 73638 - Payment success {OrderId} {SessionId}", orderId, session_id);
        return View();
    }

    [HttpGet]
    public IActionResult Cancel(string orderId)
    {
        Log.Warning("Tiago Borges 73638 - Payment cancelled {OrderId}", orderId);
        return View();
    }

}