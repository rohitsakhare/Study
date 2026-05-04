using Microsoft.EntityFrameworkCore;

namespace Shared.Messaging;

public interface IInboxDbContext
{
    DbSet<InboxMessage> InboxMessages { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
