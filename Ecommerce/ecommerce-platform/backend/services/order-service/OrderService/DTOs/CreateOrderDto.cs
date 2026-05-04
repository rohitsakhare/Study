namespace OrderService.DTOs;

public class CreateOrderDto
{
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}