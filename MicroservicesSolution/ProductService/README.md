# Product Service

The Product Service is a microservice responsible for managing product catalog data in the microservices solution. It provides full CRUD operations for product management with PostgreSQL database persistence.

## Overview

This service provides comprehensive RESTful APIs for product catalog management including creation, retrieval, updating, and deletion of products. It is built using .NET 10.0, ASP.NET Core, and Entity Framework Core with PostgreSQL.

## Features

- **Full CRUD Operations**: Create, Read, Update, Delete products
- **Advanced Filtering**: Filter by category, price range, active status, and search
- **PostgreSQL Database**: Persistent data storage with Entity Framework Core
- **RESTful API**: Clean, documented endpoints
- **Data Validation**: Comprehensive input validation
- **Error Handling**: Proper error responses and logging
- **CORS Support**: Cross-origin resource sharing enabled

## Prerequisites

- .NET 10.0 SDK
- PostgreSQL database (local or Docker)

## Database Setup

### Local PostgreSQL
```bash
# Install PostgreSQL and create database
createdb ProductDb
# Or use pgAdmin to create the database
```

### Docker PostgreSQL
```bash
docker run --name postgres-db -e POSTGRES_DB=ProductDb -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=password -p 5432:5432 -d postgres:15-alpine
```

### Initial Data
The service automatically creates all required tables and populates them with three sample products when first started:
- **Laptop** (Electronics, $1299.99)
- **Phone** (Electronics, $899.99)
- **Headphones** (Electronics, $199.99)

Subsequent application restarts will not duplicate this seed data.

## Running Locally

1. **Navigate to the ProductService directory**
   ```bash
   cd ProductService
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update database connection** (if needed)
   - Edit `appsettings.json` or `appsettings.Development.json`
   - Ensure PostgreSQL is running on `localhost:5432`

4. **Run the service** (migrations and seeding happen automatically)
   ```bash
   dotnet run
   ```

The service will start on `http://localhost:5001` (or as configured in launchSettings.json).

**Note**: Database migrations are applied automatically on startup in Program.cs, and initial products are seeded if the Products table is empty.

## API Endpoints

### Get All Products
```http
GET /api/products
```
**Query Parameters:**
- `category`: Filter by category
- `isActive`: Filter by active status (true/false)
- `minPrice`: Minimum price filter
- `maxPrice`: Maximum price filter
- `search`: Search in name and description

### Get Product by ID
```http
GET /api/products/{id}
```

### Create Product
```http
POST /api/products
Content-Type: application/json

{
  "name": "Product Name",
  "description": "Product description",
  "price": 99.99,
  "stockQuantity": 100,
  "category": "Category"
}
```

### Update Product
```http
PUT /api/products/{id}
Content-Type: application/json

{
  "name": "Updated Name",
  "price": 129.99,
  "stockQuantity": 80
}
```

### Delete Product
```http
DELETE /api/products/{id}
```

### Get Categories
```http
GET /api/products/categories
```

## Data Models

### Product Entity
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Category { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### Database Context (ApplicationDbContext)
- Handles all database operations via Entity Framework Core
- Automatically configures PostgreSQL table and column naming conventions (lowercase)
- Provides the `SeedInitialProducts()` method for dynamic data initialization
- Sets default value for `IsActive` property to `true`

### API Response
```json
{
  "id": 1,
  "name": "Laptop",
  "description": "High-performance laptop",
  "price": 1299.99,
  "stockQuantity": 50,
  "category": "Electronics",
  "isActive": true,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

**Note**: Database table is created as `products` (lowercase) and columns are also lowercase per PostgreSQL conventions.

## Docker

### Build and Run with Docker Compose
```bash
# From project root
docker-compose up --build
```

### Build Individual Image
```bash
cd ProductService
docker build -t productservice .
docker run -p 8080:8080 -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=ProductDb;Username=postgres;Password=password" productservice
```

## Testing

### Using HTTP Files
Use the `ProductService.http` file for testing endpoints in VS Code with the REST Client extension.

### Sample Requests

1. **Get all products**
   ```bash
   curl -X GET "http://localhost:5001/api/products" -H "accept: application/json"
   ```

2. **Create a product**
   ```bash
   curl -X POST "http://localhost:5001/api/products" \
        -H "Content-Type: application/json" \
        -d '{
          "name": "Test Product",
          "description": "A test product",
          "price": 19.99,
          "stockQuantity": 10,
          "category": "Test"
        }'
   ```

3. **Update a product**
   ```bash
   curl -X PUT "http://localhost:5001/api/products/1" \
        -H "Content-Type: application/json" \
        -d '{"price": 24.99}'
   ```

4. **Delete a product**
   ```bash
   curl -X DELETE "http://localhost:5001/api/products/1"
   ```

## Configuration

### Connection String
Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ProductDb;Username=postgres;Password=password"
  }
}
```

For Docker containers, use the service name as hostname:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres;Port=5432;Database=ProductDb;Username=postgres;Password=password"
  }
}
```

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Environment (Development/Production)
- `ASPNETCORE_URLS`: Server URLs (e.g., `http://0.0.0.0:8080`)
- `ConnectionStrings__DefaultConnection`: Database connection string (overrides appsettings.json)

### Automatic Features
- **Migrations**: Applied automatically on application startup
- **Seeding**: Initial products added automatically if Products table is empty
- **Naming Convention**: All table and column names are automatically lowercase for PostgreSQL compatibility

## Database Initialization

The database is automatically initialized on application startup:

### Automatic Setup Process
1. **Migrations Applied**: Entity Framework migrations are applied to create tables
2. **Dynamic Seeding**: Initial products are seeded with current UTC timestamp if the Products table is empty
3. **PostgreSQL Configuration**: Tables and columns are automatically configured to use lowercase names for PostgreSQL compatibility

### Implementation Details
```csharp
// In Program.cs
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Apply migrations
    dbContext.Database.Migrate();
    
    // Seed initial products with current UTC date/time
    dbContext.SeedInitialProducts();
}
```

The `SeedInitialProducts()` method in ApplicationDbContext checks if the Products table is empty before seeding to avoid duplicate data on subsequent application restarts.

## Database Migrations

### Create a New Migration
```bash
dotnet ef migrations add YourMigrationName
```

### Apply Pending Migrations Manually
```bash
dotnet ef database update
```

### Rollback to Previous Migration
```bash
dotnet ef database update PreviousMigrationName
```

### Remove Last Migration (before applied to database)
```bash
dotnet ef migrations remove
```

## Error Handling

The service provides comprehensive error handling:

- **400 Bad Request**: Invalid input data
- **404 Not Found**: Product not found
- **500 Internal Server Error**: Server errors (logged)

Error response format:
```json
{
  "message": "Error description",
  "details": "Additional error information"
}
```

## Logging

Logs are written to console and can be configured in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "ProductService": "Debug"
    }
  }
}
```

## Integration

This service is routed through the API Gateway at `/products`. All requests to the gateway are forwarded to this service.

## Development Guidelines

- Use meaningful commit messages
- Write unit tests for business logic
- Update API documentation for changes
- Follow RESTful conventions
- Validate all inputs
- Handle errors gracefully
- Keep methods focused and single-purpose