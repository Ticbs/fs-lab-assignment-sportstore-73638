using Serilog;
using Stripe;
using Stripe.Checkout;
using SportsStore.Service.Payments;

namespace SportsStore.Service.Payments;

public sealed class StripePaymentService : IPaymentService
{
    private readonly StripeSettings _settings;

    public StripePaymentService(StripeSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.SecretKey))
            throw new ArgumentException("Stripe SecretKey is required", nameof(settings.SecretKey));

        _settings = settings;
        StripeConfiguration.ApiKey = _settings.SecretKey;
    }

    public async Task<(string SessionId, string Url)> CreateCheckoutSessionAsync(
        decimal amount,
        string currency,
        string orderId)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than 0", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required", nameof(currency));

        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("OrderId is required", nameof(orderId));

        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = $"{_settings.SuccessUrl}?sessionId={{CHECKOUT_SESSION_ID}}&orderId={orderId}",
            CancelUrl = $"{_settings.CancelUrl}?orderId={orderId}",
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = currency.ToLower(),
                        UnitAmount = (long)(amount * 100m),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"SportsStore Order #{orderId}"
                        }
                    }
                }
            }
        };

        Log.Information(
            "Tiago Borges 73638 - Creating Stripe session | OrderId: {OrderId}, Amount: {Amount} {Currency}",
            orderId,
            amount,
            currency);

        try
        {
            var service = new SessionService();
            var session = await service.CreateAsync(options);

            Log.Information(
                "Stripe session created successfully | SessionId: {SessionId}, OrderId: {OrderId}",
                session.Id,
                orderId);

            return (session.Id, session.Url ?? string.Empty);
        }
        catch (StripeException ex)
        {
            Log.Error(
                ex,
                "Tiago Borges 73638 - Stripe session creation failed | OrderId: {OrderId}, Error: {ErrorCode}",
                orderId,
                ex.StripeError?.Code);

            throw;
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "Tiago Borges 73638 - Unexpected error creating Stripe session | OrderId: {OrderId}",
                orderId);

            throw;
        }
    }
}