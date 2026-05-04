using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Services;

public class OrderServiceImpl : IOrderService
{
    private readonly IOrderRepository _repo;

    public OrderServiceImpl(IOrderRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Order>> GetAll()
        => _repo.GetAllAsync();

    public Task<Order?> GetById(Guid id)
        => _repo.GetByIdAsync(id);

    public async Task<Order> Create(CreateOrderDto dto)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = dto.CustomerName,
            TotalAmount = dto.TotalAmount,
            Status = "Pending"
        };

        await _repo.AddAsync(order);
        return order;
    }

    public async Task<bool> Update(Guid id, UpdateOrderDto dto)
    {
        var order = await _repo.GetByIdAsync(id);
        if (order == null) return false;

        order.Status = dto.Status;
        await _repo.UpdateAsync(order);
        return true;
    }

    public async Task<bool> Delete(Guid id)
    {
        var order = await _repo.GetByIdAsync(id);
        if (order == null) return false;

        await _repo.DeleteAsync(order);
        return true;
    }
}