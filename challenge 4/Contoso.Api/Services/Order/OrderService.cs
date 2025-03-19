using AutoMapper;
using Contoso.Api.Data;
using Contoso.Api.Models;
using Contoso.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Contoso.Api.Services;

public class OrderService : IOrderService
{
    private readonly ContosoDbContext _context;
    private readonly IMapper _mapper;

    public OrderService(ContosoDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<OrderDto>> GetOrdersAsync(int userId)
    {
         var orders = await _context.Orders
                            .Where(o => o.UserId == userId)
                            .ToListAsync();

        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
                await _context.Entry(item)
                    .Reference(i => i.Product)
                    .LoadAsync();
            }
        }

        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto> GetOrderByIdAsync(int id, int userId)
    {
        var order = await _context.Orders
                            .Where(o => o.UserId == userId && o.Id == id)
                            .FirstOrDefaultAsync();
        
        foreach (var item in order.Items)
        {
            await _context.Entry(item)
                .Reference(i => i.Product)
                .LoadAsync();
        }

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto)
    {
        var newOrder = new Order
        {
            UserId = orderDto.UserId,
            Total = orderDto.Total,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
        };

        var orderItemCollection = new List<OrderItem>();

        if (orderDto.Items != null)
        {
            foreach (var orderItem in orderDto.Items)
            {
                var newOrderItem = new OrderItem
                {
                    OrderId = newOrder.Id,
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.Price
                };

                orderItemCollection.Add(newOrderItem);      
            }    
        }

        newOrder.Items = orderItemCollection;

        await _context.Orders.AddAsync(newOrder);

        await _context.SaveChangesAsync();

        return _mapper.Map<OrderDto>(newOrder);     
    }
}