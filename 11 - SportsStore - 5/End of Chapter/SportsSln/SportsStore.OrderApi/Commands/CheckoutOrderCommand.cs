using MediatR;
using SportsStore.OrderApi.Models;

namespace SportsStore.OrderApi.Commands;

public class CheckoutOrderCommand : IRequest<Order>
{
    public string CustomerId { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
