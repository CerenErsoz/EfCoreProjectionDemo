using AutoMapper;
using EfCoreProjectionDemo.Data.Entities;
using EfCoreProjectionDemo.DTOs;

namespace EfCoreProjectionDemo.Profiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        // Customer mapping
        CreateMap<Customer, CustomerDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"));

        // Product mapping
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));

        // OrderItem mapping
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Product.Category.Name));

        // Order List mapping (Flattening and Aggregation)
        CreateMap<Order, OrderListDto>()
            .ForMember(d => d.CustomerFullName, opt => opt.MapFrom(s => $"{s.Customer.FirstName} {s.Customer.LastName}"))
            .ForMember(d => d.ProductCount, opt => opt.MapFrom(s => s.OrderItems.Sum(oi => oi.Quantity)))
            .ForMember(d => d.TotalPrice, opt => opt.MapFrom(s => s.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)));

        // Order Detail mapping (Nested mapping and Aggregation)
        CreateMap<Order, OrderDetailDto>()
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.OrderItems))
            .ForMember(d => d.TotalPrice, opt => opt.MapFrom(s => s.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)));
    }
}
