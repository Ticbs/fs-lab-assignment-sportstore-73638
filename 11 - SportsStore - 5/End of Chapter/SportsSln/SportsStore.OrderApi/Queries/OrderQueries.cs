using MediatR;
using SportsStore.OrderApi.Models;

namespace SportsStore.OrderApi.Queries;

public class GetAllOrdersQuery : IRequest<List<Order>>
{
}

public class GetOrderByIdQuery : IRequest<Order?>
{
    public Guid OrderId { get; set; }
}

public class GetOrderStatusQuery : IRequest<string?>
{
    public Guid OrderId { get; set; }
}
