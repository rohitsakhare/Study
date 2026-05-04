using ProductReadService.Application.Handlers;
using ProductReadService.Infrastructure.Data;
using ProductReadService.Infrastructure.Kafka;
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


// ---------------------
// Dependency Injection
// ---------------------
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetProductsHandler).Assembly);
});

builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddHostedService<ProductConsumer>();
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

// DB Migration (startup)


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