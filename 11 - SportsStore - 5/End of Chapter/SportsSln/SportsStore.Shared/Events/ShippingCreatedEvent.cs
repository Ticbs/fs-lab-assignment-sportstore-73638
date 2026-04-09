namespace SportsStore.Shared.Events;
public class ShippingCreatedEvent
{
    public Guid OrderId { get; set; }
    public bool Success { get; set; }
    public string TrackingReference { get; set; } = string.Empty;
    public DateTime EstimatedDispatch { get; set; }
    public string Message { get; set; } = string.Empty;
}
