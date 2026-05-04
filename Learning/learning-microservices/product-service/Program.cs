using Microsoft.EntityFrameworkCore;
using product_service.Data;
using product_service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();



builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<RabbitMqService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// ------------------------
// Apply migrations automatically
// ------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.Migrate();  // creates Products table if it doesn't exist
}

// ------------------------
// Middleware
// ------------------------
app.UseCors();             // Must come BEFORE MapControllers

app.MapControllers();

// Run the app
app.Run();
