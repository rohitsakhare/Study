# API Gateway

The API Gateway serves as the centralized entry point for all client requests in the microservices architecture. Built with ASP.NET Core and Ocelot, it provides request routing, load balancing, and protocol translation to backend microservices.

## Overview

This API Gateway implements the API Gateway Pattern, providing a single unified interface for clients to interact with multiple backend services. It handles all request routing, authentication-ready integration, and service discovery configuration.

## Features

- **Centralized Routing**: Route all client requests to appropriate backend services
- **Service Abstraction**: Hide backend service details from clients
- **Protocol Translation**: Convert between different communication protocols if needed
- **Load Balancing Ready**: Support for routing to multiple service instances
- **Authentication Ready**: Framework for implementing centralized authentication
- **CORS Support**: Handle cross-origin requests
- **Configuration File-Based**: Easy route management via `ocelot.json`
- **Health Checks**: Foundation for service health monitoring

## Architecture

```
┌─────────────────────────────────────┐
│          External Clients           │
└────────────────────┬────────────────┘
                     │
                     ▼
    ┌────────────────────────────────┐
    │      API Gateway (Port 8000)   │
    │  ┌──────────────────────────┐  │
    │  │   Ocelot Router Engine   │  │
    │  │  • Request Routing       │  │
    │  │  • Load Balancing        │  │
    │  │  • Service Discovery     │  │
    │  └──────────────────────────┘  │
    └───────────┬──────────┬─────────┘
                │          │
      ┌─────────▼──┐  ┌────▼─────────┐
      │  UserSvc   │  │ ProductSvc   │  ...other services
      │(Port 5000) │  │ (Port 5001)  │
      └────────────┘  └──────────────┘
```

## Prerequisites

- .NET 10.0 SDK
- Ocelot NuGet package (included in project)

## Running Locally

### Start API Gateway Only
```bash
cd ApiGateway
dotnet run
```
Gateway will be available at `http://localhost:8000`

**Prerequisites**: Ensure backend services are running on their configured ports:
- User Service: `http://localhost:5000`
- Product Service: `http://localhost:5001`
- Order Service: `http://localhost:5002`

### Start with Docker Compose (Recommended)
```bash
# From project root directory
docker-compose up --build
```
Gateway will be available at `http://localhost:8000`

### Available Routes

| Method | Endpoint | Backend | Description |
|--------|----------|---------|-------------|
| GET | `/users` | User Service | List all users |
| GET | `/products` | Product Service | List all products (supports filtering) |
| POST | `/products` | Product Service | Create new product |
| GET | `/products/{id}` | Product Service | Get product by ID |
| PUT | `/products/{id}` | Product Service | Update product |
| DELETE | `/products/{id}` | Product Service | Delete product |
| GET | `/products/categories` | Product Service | Get unique product categories |
| GET | `/orders` | Order Service | List all orders |

## Configuration

### Ocelot.json Routing Configuration

The `ocelot.json` file defines all route mappings from external endpoints to internal backend services:

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/users",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [{ "Host": "userservice", "Port": 8080 }],
      "UpstreamPathTemplate": "/users",
      "UpstreamHttpMethod": [ "GET" ]
    },
    {
      "DownstreamPathTemplate": "/api/products",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [{ "Host": "productservice", "Port": 8080 }],
      "UpstreamPathTemplate": "/products",
      "UpstreamHttpMethod": [ "GET", "POST" ]
    },
    {
      "DownstreamPathTemplate": "/api/products/{id}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [{ "Host": "productservice", "Port": 8080 }],
      "UpstreamPathTemplate": "/products/{id}",
      "UpstreamHttpMethod": [ "GET", "PUT", "DELETE" ]
    },
    {
      "DownstreamPathTemplate": "/api/products/categories",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [{ "Host": "productservice", "Port": 8080 }],
      "UpstreamPathTemplate": "/products/categories",
      "UpstreamHttpMethod": [ "GET" ]
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://apigateway:8080"
  }
}
```

### Route Configuration Properties

- **DownstreamPathTemplate**: Path on the backend service
- **DownstreamScheme**: Protocol for backend communication (http/https)
- **DownstreamHostAndPorts**: Backend service location and port
- **UpstreamPathTemplate**: External path clients use
- **UpstreamHttpMethod**: HTTP methods to accept (GET, POST, PUT, DELETE, etc.)

**Important Route Ordering**: More specific routes must appear BEFORE generic routes in the routes array. For example:
- `/products/{id}` must come before `/products`
- `/products/categories` must come before `/products`

This ensures Ocelot matches the correct route template.

### Adding New Routes

To add a new route, add an entry to the `Routes` array in `ocelot.json`. **Important**: For routes with path parameters like `{id}`, create a separate route entry:

**Example: Adding a new endpoint with ID parameter**
```json
{
  "DownstreamPathTemplate": "/api/newservice/{id}",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [{ "Host": "newservice", "Port": 8080 }],
  "UpstreamPathTemplate": "/newservice/{id}",
  "UpstreamHttpMethod": [ "GET", "PUT", "DELETE" ]
}
```

**Example: Adding a collection endpoint**
```json
{
  "DownstreamPathTemplate": "/api/newservice",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [{ "Host": "newservice", "Port": 8080 }],
  "UpstreamPathTemplate": "/newservice",
  "UpstreamHttpMethod": [ "GET", "POST" ]
}
```

## Environment-Specific Configuration

### Local Development
- Backend services run on `localhost` with individual ports (5000-5002)
- Gateway runs on `http://localhost:8000`

### Docker
- Services communicate via service names defined in `docker-compose.yml`
- Gateway runs on `http://0.0.0.0:8080` internally
- Exposed on `http://localhost:8000` externally

### Appsettings Configuration

**appsettings.json** (shared):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**appsettings.Development.json** (override for dev):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

## API Examples

### Get All Users
```bash
curl "http://localhost:8000/users"
```

### Product Service - Complete CRUD Examples

#### List All Products
```bash
curl "http://localhost:8000/products"
```

#### List Products with Filters
```bash
# Filter by category, price range, and active status
curl "http://localhost:8000/products?category=Electronics&minPrice=100&maxPrice=1500&isActive=true"

# Search products
curl "http://localhost:8000/products?search=laptop"
```

#### Get Specific Product
```bash
curl "http://localhost:8000/products/1"
```

#### Create Product
```bash
curl -X POST "http://localhost:8000/products" \
     -H "Content-Type: application/json" \
     -d '{
       "name": "Wireless Keyboard",
       "description": "Mechanical wireless keyboard",
       "price": 89.99,
       "stockQuantity": 150,
       "category": "Electronics"
     }'
```

#### Update Product (Full)
```bash
curl -X PUT "http://localhost:8000/products/1" \
     -H "Content-Type: application/json" \
     -d '{
       "name": "Wireless Keyboard Pro",
       "description": "Mechanical wireless keyboard with RGB",
       "price": 129.99,
       "stockQuantity": 120,
       "category": "Electronics"
     }'
```

#### Update Product (Partial)
```bash
curl -X PUT "http://localhost:8000/products/1" \
     -H "Content-Type: application/json" \
     -d '{"price": 79.99, "stockQuantity": 120}'
```

#### Delete Product
```bash
curl -X DELETE "http://localhost:8000/products/1"
```

#### Get All Product Categories
```bash
curl "http://localhost:8000/products/categories"
```

### Get All Orders
```bash
curl "http://localhost:8000/orders"
```

## Testing

### HTTP Files
Use the `ApiGateway.http` file for testing endpoints in VS Code with the REST Client extension:

1. Install the REST Client extension in VS Code
2. Open `ApiGateway.http`
3. Click "Send Request" above each request to test

### Manual Testing
```bash
# Test gateway health by accessing any route
curl http://localhost:8000/users -v

# Check response to see routing is working
```

### Verify Backend Services
Before testing gateway routes, ensure backend services are running:

```bash
# Local development
curl http://localhost:5000/api/users                  # User Service
curl http://localhost:5001/api/products               # Product Service
curl http://localhost:5001/api/products/1             # Product Service - Get by ID
curl http://localhost:5001/api/products/categories    # Product Service - Categories
curl http://localhost:5002/api/orders                 # Order Service
```

## Logging

Ocelot logs all routing decisions and errors to the console. Configure log levels in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Ocelot": "Debug",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Troubleshooting

### Gateway Not Routing to Services
- **Check**: Verify backend services are running on configured ports
- **Check**: Verify `ocelot.json` routes are correctly configured
- **Check**: In Docker, ensure services use correct service names (e.g., `userservice`, not `localhost`)

### Service Not Found Errors
```
Ocelot.Errors: Unable to find service
```
- **Solution**: Verify `DownstreamHostAndPorts` in `ocelot.json`
- **Solution**: For Docker, use service names from `docker-compose.yml`

### Port Already in Use
```
System.Net.Sockets.SocketException: Address already in use
```
- **Solution**: Change port in `launchSettings.json` or `docker-compose.yml`
- **Solution**: Kill process using port: `netstat -ano | findstr :8000` then `taskkill /PID <PID>`

### Docker Container Won't Start
- **Check**: All backend services are defined in `docker-compose.yml`
- **Check**: Service names in `ocelot.json` match `docker-compose.yml` service names
- **View Logs**: `docker-compose logs apigateway`

## Performance Optimization

### Load Balancing Configuration
To distribute requests across multiple instances, add multiple hosts in a route:

```json
{
  "DownstreamPathTemplate": "/api/products",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    { "Host": "productservice-1", "Port": 8080 },
    { "Host": "productservice-2", "Port": 8080 },
    { "Host": "productservice-3", "Port": 8080 }
  ],
  "UpstreamPathTemplate": "/products",
  "UpstreamHttpMethod": [ "GET", "POST" ]
}
```

Ocelot will automatically load balance requests across all configured instances using round-robin distribution.

### Caching (Advanced)
Future enhancement: Add response caching for GET requests to improve performance.

## Integration

The API Gateway is the entry point for all external traffic. All microservices should be hidden behind the gateway:

```
Clients → API Gateway:8000 → Backend Services
```

This pattern provides:
- Single point of entry for clients
- Service locations hidden from clients
- Centralized management of API changes
- Potential for authentication, logging, rate limiting (future enhancements)

## Development Guidelines

- **Test Route Changes**: Always test new routes locally before deploying
- **Update Documentation**: Update this README when adding/removing routes
- **Use Meaningful Paths**: Keep upstream paths consistent and intuitive
- **Error Handling**: Ocelot automatically returns 502/503 if backend unavailable
- **Security**: Plan for authentication and authorization middleware in future versions

## Future Enhancements

- [ ] Authentication/Authorization middleware
- [ ] Rate limiting
- [ ] Request/Response transformation
- [ ] Request correlation IDs
- [ ] Per-route caching policies
- [ ] Service discovery from Consul/Kubernetes
- [ ] Metrics and monitoring integration
- [ ] Request timeout policies

## Resources

- [Ocelot Documentation](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-resilience-using-exponential-backoff)
- [API Gateway Pattern](https://microservices.io/patterns/apigateway.html)
- [ASP.NET Core Routing](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing)

## Support

For issues or questions:
1. Check Ocelot logs: `docker-compose logs apigateway`
2. Verify backend service connectivity
3. Review route configuration in `ocelot.json`
4. Check service is running on expected port
