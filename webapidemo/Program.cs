using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;

// In-memory orders store
var orders = new ConcurrentDictionary<int, string>();
var builder = WebApplication.CreateBuilder(args);

// Structured logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
    options.IncludeScopes = true;
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.WriteIndented = true;
});

var app = builder.Build();

// Health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

// Orders endpoints
app.MapGet("/orders", () => Results.Ok(orders.Values));

app.MapPost("/orders", (string order) =>
{
    var id = orders.Count + 1;
    orders[id] = order;
    return Results.Created($"/orders/{id}", order);
});

app.MapGet("/orders/{id:int}", (int id) =>
{
    return orders.TryGetValue(id, out var order)
        ? Results.Ok(order)
        : Results.NotFound();
});

// Metrics endpoint in Prometheus format
app.MapGet("/metrics", () =>
{
    var sb = new StringBuilder();
    sb.AppendLine("# HELP orders_total Total number of orders");
    sb.AppendLine("# TYPE orders_total counter");
    sb.AppendLine($"orders_total {orders.Count}");
    return Results.Text(sb.ToString(), "text/plain; version=0.0.4");
});

app.Run();
