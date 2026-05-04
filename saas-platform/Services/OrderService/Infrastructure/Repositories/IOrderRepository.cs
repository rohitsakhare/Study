using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetOrderByIdAsync(int id);
    Task<List<Order>> GetOrdersByUserIdAsync(int userId);
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> UpdateOrderAsync(Order order);
    Task<bool> DeleteOrderAsync(int id);
    Task<OrderItem> AddOrderItemAsync(OrderItem orderItem);
    Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
}
