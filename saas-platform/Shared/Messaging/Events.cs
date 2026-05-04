namespace Shared.Messaging;

public interface IMessage
{
    string MessageId { get; }
    DateTime Timestamp { get; }
}

public abstract class BaseMessage : IMessage
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// Event Messages
public class UserCreatedEvent : BaseMessage
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class UserUpdatedEvent : BaseMessage
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class UserDeletedEvent : BaseMessage
{
    public int UserId { get; set; }
}

public class OrderCreatedEvent : BaseMessage
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
}

public class OrderShippedEvent : BaseMessage
{
    public int OrderId { get; set; }
    public string TrackingNumber { get; set; } = null!;
}

public class ProductCreatedEvent : BaseMessage
{
    public string ProductId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}

public class ProductUpdatedEvent : BaseMessage
{
    public string ProductId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}
