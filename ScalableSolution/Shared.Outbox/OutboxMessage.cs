namespace Shared.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public string Status { get; set; } = "Pending";
    public DateTime OccurredOn { get; set; }
    public DateTime? ProcessedOn { get; set; }


    public int RetryCount { get; set; }
    public string? LastError { get; set; }
    public Guid CorrelationId { get; set; }
}
