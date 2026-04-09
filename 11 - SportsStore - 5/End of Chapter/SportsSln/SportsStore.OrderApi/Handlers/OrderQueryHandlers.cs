using MediatR;
using Microsoft.EntityFrameworkCore;
using SportsStore.OrderApi.Data;
using SportsStore.OrderApi.Models;
using SportsStore.OrderApi.Queries;

namespace SportsStore.OrderApi.Handlers;

public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, List<Order>>
{
    private readonly OrderDbContext _db;

    public GetAllOrdersHandler(OrderDbContext db)
    {
        _db = db;
    }

    public async Task<List<Order>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _db.Orders.Include(o => o.Items).ToListAsync(cancellationToken);
    }
}

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Order?>
{
    private readonly OrderDbContext _db;

    public GetOrderByIdHandler(OrderDbContext db)
    {
        _db = db;
    }

    public async Task<Order?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await _db.Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
    }
}

public class GetOrderStatusHandler : IRequestHandler<GetOrderStatusQuery, string?>
{
    private readonly OrderDbContext _db;

    public GetOrderStatusHandler(OrderDbContext db)
    {
        _db = db;
    }

    public async Task<string?> Handle(GetOrderStatusQuery request, CancellationToken cancellationToken)
    {
        var order = await _db.Orders.FindAsync(request.OrderId);
        return order?.Status;
    }
}
