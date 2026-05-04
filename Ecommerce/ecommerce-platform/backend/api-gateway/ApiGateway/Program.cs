using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AspNetCoreRateLimit;
using Serilog;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ── Logging ─────────────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ── Rate Limiting ─────────────────────────────────────────────────────────────
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(
    builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ── Authentication (Keycloak JWT) ─────────────────────────────────────────────
var jwtConfig = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = jwtConfig["Authority"];
        options.Audience  = jwtConfig["Audience"];
        options.RequireHttpsMetadata = bool.Parse(jwtConfig["RequireHttpsMetadata"]!);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType            = "roles",     // matches mapper from Phase 3
            NameClaimType            = "preferred_username"
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Log.Warning("JWT auth failed: {Error}", ctx.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                var username = ctx.Principal?.FindFirstValue("preferred_username");
                Log.Information("Token validated for user: {User}", username);
                return Task.CompletedTask;
            }
        };
    });

// ── Authorization Policies ─────────────────────────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    // Any valid JWT — just needs to be logged in
    options.AddPolicy("authenticated", policy =>
        policy.RequireAuthenticatedUser());

    // Only ADMIN role
    options.AddPolicy("admin-only", policy =>
        policy.RequireRole("ADMIN"));

    // Delivery agents or admins
    options.AddPolicy("delivery-or-admin", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole("DELIVERY_AGENT") || ctx.User.IsInRole("ADMIN")));
});

// ── YARP Reverse Proxy ─────────────────────────────────────────────────────────
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(t =>
    {
        t.RequestTransforms.Add(new UserHeaderTransform());
    });

// ── Health Checks ──────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks();

// ═══════════════════════════════════════════════════════════════════════════════
var app = builder.Build();
// ═══════════════════════════════════════════════════════════════════════════════

app.UseIpRateLimiting();
app.UseCors("ReactApp");
app.UseHttpsRedirection();

// Serilog request logging — logs every request with duration
app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
    {
        diagCtx.Set("UserId",     httpCtx.User.FindFirstValue("sub") ?? "anonymous");
        diagCtx.Set("Username",   httpCtx.User.FindFirstValue("preferred_username") ?? "anonymous");
        diagCtx.Set("RemoteIp",   httpCtx.Connection.RemoteIpAddress?.ToString());
    };
});

app.UseRouting();
app.UseAuthentication();  // order matters: auth before authorization
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapReverseProxy();    // YARP handles all /api/** routes

app.Run();