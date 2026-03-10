namespace SportsStore.Service.Payments;

public interface IPaymentService
{
    Task<(string SessionId, string Url)> CreateCheckoutSessionAsync(decimal amount, string currency, string orderId);
}