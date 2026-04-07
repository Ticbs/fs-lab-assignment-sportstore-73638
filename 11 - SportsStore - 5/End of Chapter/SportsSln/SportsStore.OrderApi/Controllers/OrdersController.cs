using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore.OrderApi.Data;
using SportsStore.OrderApi.Models;
using SportsStore.OrderApi.Services;
using SportsStore.Shared.Events;

namespace SportsStore.OrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _db;
    private readonly RabbitMQPublisher _publisher;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(OrderDbContext db, RabbitMQPublisher publisher,
        ILogger<OrdersController> logger)
    {
        _db = db;
        _publisher = publisher;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _db.Orders.Include(o => o.Items).ToListAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();
        return Ok(new { order.Id, order.Status });
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
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
        await _db.SaveChangesAsync();

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

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();
        order.Status = request.Status;
        await _db.SaveChangesAsync();
        return Ok(order);
    }
}

public class CheckoutRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public List<CheckoutItem> Items { get; set; } = new();
}

public class CheckoutItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
