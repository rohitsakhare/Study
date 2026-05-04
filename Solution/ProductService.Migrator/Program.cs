using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;

var services = new ServiceCollection();

// DB configuration (same as ProductService)
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    )
);

var provider = services.BuildServiceProvider();

using (var scope = provider.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    Console.WriteLine("🔄 Applying ProductService migrations...");

    db.Database.Migrate();

    Console.WriteLine("✅ ProductService migrations applied successfully.");
}