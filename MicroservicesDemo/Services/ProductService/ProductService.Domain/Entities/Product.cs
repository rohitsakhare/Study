namespace ProductService.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Stock { get; set; }
}
