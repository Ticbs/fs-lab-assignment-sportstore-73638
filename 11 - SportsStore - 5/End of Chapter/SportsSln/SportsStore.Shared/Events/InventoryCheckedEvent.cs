namespace SportsStore.Shared.Events;
public class InventoryCheckedEvent
{
    public Guid OrderId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
