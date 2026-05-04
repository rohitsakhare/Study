using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Repositories;
using OrderService.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderServiceImpl>();

// 👇 Enable OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Enable Swagger UI
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderService API v1");
//         c.RoutePrefix = string.Empty; // Swagger at root: http://localhost:xxxx/
//     });
// }

app.UseAuthorization();

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.Title = "Order Service API";
    options.Theme = ScalarTheme.Default;
});

app.MapControllers();



app.Run();