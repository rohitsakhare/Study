using OrderService.DTOs;
using OrderService.Models;

namespace OrderService.Services;

public interface IOrderService
{
    Task<List<Order>> GetAll();
    Task<Order?> GetById(Guid id);
    Task<Order> Create(CreateOrderDto dto);
    Task<bool> Update(Guid id, UpdateOrderDto dto);
    Task<bool> Delete(Guid id);
}