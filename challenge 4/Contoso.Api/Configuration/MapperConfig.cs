using AutoMapper;
using Contoso.Api.Data;
using Contoso.Api.Models;

namespace Contoso.Api.Configuration;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Product, ProductDto>().ReverseMap();

        CreateMap<Order, OrderDto>().ReverseMap();

        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
    }
}