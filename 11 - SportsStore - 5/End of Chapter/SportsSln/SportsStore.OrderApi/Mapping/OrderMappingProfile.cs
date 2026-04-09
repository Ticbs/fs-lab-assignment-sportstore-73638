using AutoMapper;
using SportsStore.OrderApi.DTOs;
using SportsStore.OrderApi.Models;

namespace SportsStore.OrderApi.Mapping;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
    }
}
