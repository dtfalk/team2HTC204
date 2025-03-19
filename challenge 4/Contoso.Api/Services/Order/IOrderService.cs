using Contoso.Api.Data;

namespace Contoso.Api.Services;

 
 public interface IOrderService
 {
    Task<IEnumerable<OrderDto>> GetOrdersAsync(int userId);

    Task<OrderDto> GetOrderByIdAsync(int id, int userId);

    Task<OrderDto> CreateOrderAsync(OrderDto orderDto);

 }