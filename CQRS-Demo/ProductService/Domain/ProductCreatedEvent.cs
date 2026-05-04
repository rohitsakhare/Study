namespace ProductService.Domain;

public class ProductCreatedEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}