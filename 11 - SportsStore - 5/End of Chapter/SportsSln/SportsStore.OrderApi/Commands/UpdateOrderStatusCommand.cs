using MediatR;
using SportsStore.OrderApi.Models;

namespace SportsStore.OrderApi.Commands;

public class UpdateOrderStatusCommand : IRequest<Order?>
{
    public Guid OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
}
