namespace PaymentService.Models;

public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Succeeded, Failed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
