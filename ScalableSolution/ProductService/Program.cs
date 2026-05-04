using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductService.Application.Services;
using ProductService.Infrastructure.Data;
using Scalar.AspNetCore;
using Shared.Messaging;
using Shared.Messaging.Abstractions;
using Shared.Messaging.Kafka;
using Shared.Outbox;

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
// DB (PostgreSQL)
// ---------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);


builder.Services.AddScoped<IOutboxDbContext>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IInboxDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// ---------------------
// Dependency Injection
// ---------------------
//builder.Services.AddSingleton<KafkaTopicInitializer>();
builder.Services.AddScoped<IProductServiceContract, ProductServiceImplementation>();
// builder.Services.AddSingleton<IEventBus, EventBus>();
// builder.Services.AddScoped<IOutboxHandler, ProductCreatedHandler>();
// builder.Services.AddScoped<IOutboxHandler, ProductUpdatedHandler>();
// builder.Services.AddScoped<IOutboxHandler, ProductDeletedHandler>();
// builder.Services.AddHostedService<OutboxProcessor>();

// ---------------------
// Authentication (Keycloak / JWT)
// ---------------------
var keycloakAuthority = builder.Configuration["Jwt:Authority"];

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = keycloakAuthority;
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false
        };
    });

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOnly", policy =>
        policy.RequireRole("user"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("admin"));
});

// ---------------------
// Swagger / OpenAPI
// ---------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // 🔐 JWT Security Definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    // 🔐 Global security requirement (apply to all endpoints)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Scalar OpenAPI support
builder.Services.AddOpenApi();

#endregion

var app = builder.Build();

#region ===================== PIPELINE =====================

// 🔥 AUTO CREATE TOPICS AT STARTUP
// using (var scope = app.Services.CreateScope())
// {
//     var initializer = scope.ServiceProvider.GetRequiredService<KafkaTopicInitializer>();
//     await initializer.EnsureTopicsExistAsync();
// }

// CORS (must be early)
app.UseCors("Cors");

// DB Migration (startup)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    Console.WriteLine("🔄 Applying ProductService migrations...");
    db.Database.Migrate();
    Console.WriteLine("✅ ProductService migrations applied successfully.");
}

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

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