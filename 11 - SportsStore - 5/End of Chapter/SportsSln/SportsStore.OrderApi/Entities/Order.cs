using SportsStore.Shared.Enums;
using SportsStore.Shared.Models;

namespace SportsStore.OrderApi.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}