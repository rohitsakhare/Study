using Microsoft.EntityFrameworkCore;
using ProductService;
using ProductService.Application.Handlers;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Kafka;
using ProductService.Infrastructure.Outbox;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

#region ===================== SERVICES =====================

// Controllers
builder.Services.AddControllers();

// ---------------------
// CORS
// ---------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("Cors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---------------------
// DB
// ---------------------
builder.Services.AddDbContext<WriteDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// ---------------------
// Dependency Injection
// ---------------------
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateProductHandler).Assembly);
});
builder.Services.AddSingleton<KafkaProducer>();
builder.Services.AddHostedService<OutboxWorker>();
// ---------------------
// Swagger / OpenAPI
// ---------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Scalar OpenAPI support
builder.Services.AddOpenApi();

#endregion

var app = builder.Build();

#region ===================== PIPELINE =====================

// CORS (must be early)
app.UseCors("Cors");

// Auto DB init


// Controllers
app.MapControllers();

#endregion

#region ===================== API DOCS =====================

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API");
});

// Scalar UI
app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.Title = "Product API";
    options.Theme = ScalarTheme.Default;
});

#endregion

app.Run();