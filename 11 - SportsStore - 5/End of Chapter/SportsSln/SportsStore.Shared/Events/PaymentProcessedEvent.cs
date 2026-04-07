namespace SportsStore.Shared.Events;
public class PaymentProcessedEvent
{
    public Guid OrderId { get; set; }
    public bool Success { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
