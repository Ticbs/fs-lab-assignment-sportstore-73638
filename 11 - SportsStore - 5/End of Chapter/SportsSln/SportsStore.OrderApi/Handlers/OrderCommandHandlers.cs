using MediatR;
using Microsoft.EntityFrameworkCore;
using SportsStore.OrderApi.Commands;
using SportsStore.OrderApi.Data;
using SportsStore.OrderApi.Models;
using SportsStore.OrderApi.Services;
using SportsStore.Shared.Events;

namespace SportsStore.OrderApi.Handlers;

public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, Order>
{
    private readonly OrderDbContext _db;
    private readonly RabbitMQPublisher _publisher;
    private readonly ILogger<CheckoutOrderHandler> _logger;

    public CheckoutOrderHandler(OrderDbContext db, RabbitMQPublisher publisher,
        ILogger<CheckoutOrderHandler> logger)
    {
        _db = db;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Order> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            CustomerId = request.CustomerId,
            TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice),
            Status = "Submitted",
            Items = request.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(cancellationToken);

        var evt = new OrderSubmittedEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(i => new OrderItemEvent
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        await _publisher.PublishAsync("inventory-check", evt);

        _logger.LogInformation("Order {OrderId} submitted for customer {CustomerId}",
            order.Id, order.CustomerId);

        return order;
    }
}

public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, Order?>
{
    private readonly OrderDbContext _db;
    private readonly ILogger<UpdateOrderStatusHandler> _logger;

    public UpdateOrderStatusHandler(OrderDbContext db, ILogger<UpdateOrderStatusHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Order?> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _db.Orders.FindAsync(request.OrderId);
        if (order == null) return null;

        order.Status = request.Status;
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} status updated to {Status}", request.OrderId, request.Status);

        return order;
    }
}
