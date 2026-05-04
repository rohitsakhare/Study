namespace Shared.Messaging;

public class InboxMessage
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string EventType { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTime ReceivedOn { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedOn { get; set; }
    public string Status { get; set; } = "Pending";
    public int RetryCount { get; set; }
    public string? LastError { get; set; }
}
