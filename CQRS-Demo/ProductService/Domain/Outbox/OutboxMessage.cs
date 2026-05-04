namespace ProductService.Domain.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }
    public bool Processed { get; set; }
}
