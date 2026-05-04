using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add Ocelot services
builder.Services.AddOcelot(builder.Configuration);

// Optional: console logging
builder.Logging.AddConsole();

var app = builder.Build();

// Start Ocelot and keep the app running
await app.UseOcelot();

app.Run();