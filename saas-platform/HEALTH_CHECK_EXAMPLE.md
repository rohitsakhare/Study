// Sample health check endpoint to implement in each service
// Add this to your Program.cs:

var builder = WebApplication.CreateBuilder(args);

// ... other configurations ...

builder.Services.AddHealthChecks()
    .AddDbContextCheck<UserDbContext>()
    .AddCheck("UserService", new HealthCheck("User Service is healthy"));

var app = builder.Build();

// ... other middleware ...

app.MapHealthChecks("/health");

// Health check implementation
public class HealthCheck : IHealthCheck
{
    private readonly string _name;

    public HealthCheck(string name)
    {
        _name = name;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy(_name));
    }
}
