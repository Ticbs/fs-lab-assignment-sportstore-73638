namespace SportsStore.Shared.Models;

public class OrderSubmittedEvent
{
    public Guid OrderId { get; set; }

    public string CustomerId { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
}