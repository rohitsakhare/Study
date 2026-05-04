using OrderService.Domain.Entities;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Application.Services;

public interface IOrderService
{
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId);
    Task<List<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
    Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto);
    Task<bool> DeleteOrderAsync(int id);
}

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null) return null;

        var items = await _orderRepository.GetOrderItemsByOrderIdAsync(id);
        return MapToDto(order, items);
    }

    public async Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId)
    {
        var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
        var dtos = new List<OrderDto>();

        foreach (var order in orders)
        {
            var items = await _orderRepository.GetOrderItemsByOrderIdAsync(order.Id);
            dtos.Add(MapToDto(order, items));
        }

        return dtos;
    }

    public async Task<List<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllOrdersAsync();
        var dtos = new List<OrderDto>();

        foreach (var order in orders)
        {
            var items = await _orderRepository.GetOrderItemsByOrderIdAsync(order.Id);
            dtos.Add(MapToDto(order, items));
        }

        return dtos;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        var order = new Order
        {
            UserId = createOrderDto.UserId,
            TotalAmount = createOrderDto.Items.Sum(i => i.Quantity * i.UnitPrice),
            Status = "Pending"
        };

        var createdOrder = await _orderRepository.CreateOrderAsync(order);

        foreach (var itemDto in createOrderDto.Items)
        {
            await _orderRepository.AddOrderItemAsync(new OrderItem
            {
                OrderId = createdOrder.Id,
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice
            });
        }

        var items = await _orderRepository.GetOrderItemsByOrderIdAsync(createdOrder.Id);
        return MapToDto(createdOrder, items);
    }

    public async Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
    {
        var order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null)
            throw new KeyNotFoundException($"Order with id {id} not found");

        order.Status = updateOrderDto.Status ?? order.Status;
        var updatedOrder = await _orderRepository.UpdateOrderAsync(order);

        var items = await _orderRepository.GetOrderItemsByOrderIdAsync(id);
        return MapToDto(updatedOrder, items);
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        return await _orderRepository.DeleteOrderAsync(id);
    }

    private static OrderDto MapToDto(Order order, List<OrderItem> items)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Items = items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = null!;
    public List<OrderItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateOrderDto
{
    public int UserId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class UpdateOrderDto
{
    public string? Status { get; set; }
}
