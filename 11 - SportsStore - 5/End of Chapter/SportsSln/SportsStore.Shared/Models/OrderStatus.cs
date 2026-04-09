namespace SportsStore.Shared.Models;

public enum OrderStatus
{
    Cart,
    Submitted,
    InventoryPending,
    InventoryConfirmed,
    InventoryFailed,
    PaymentPending,
    PaymentApproved,
    PaymentFailed,
    ShippingPending,
    ShippingCreated,
    Completed,
    Failed
}