using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    public class PaymentController : Controller
    {
        private readonly Cart _cart;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(Cart cartService, ILogger<PaymentController> logger)
        {
            _cart = cartService;
            _logger = logger;
        }

        public IActionResult Checkout()
        {
            if (!_cart.Lines.Any())
            {
                TempData["StripeError"] = "Your cart is empty.";
                return RedirectToAction("Checkout", "Order");
            }

            var domain = $"{Request.Scheme}://{Request.Host}/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + "Payment/Success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "Payment/Cancel",
                Mode = "payment",
                LineItems = _cart.Lines.Select(line => new SessionLineItemOptions
                {
                    Quantity = line.Quantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "eur",
                        UnitAmountDecimal = (decimal)(line.Product.Price * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = line.Product.Name
                        }
                    }
                }).ToList()
            };

            try
            {
                var service = new SessionService();
                Session session = service.Create(options);

                _logger.LogInformation("Stripe session created successfully. SessionId: {SessionId}", session.Id);

                return Redirect(session.Url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stripe checkout session");
                TempData["StripeError"] = ex.Message;
                return RedirectToAction("Checkout", "Order");
            }
        }

        public IActionResult Success(string session_id)
        {
            ViewBag.SessionId = session_id;
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}