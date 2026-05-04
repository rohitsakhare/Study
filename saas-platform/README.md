# SaaS Platform - .NET 8 Microservices

A production-ready .NET 8 microservices architecture with clean architecture principles, API Gateway, JWT authentication, and multi-database support.

## Project Structure

```
saas-platform/
├── ApiGateway/              # Ocelot API Gateway
├── Services/
│   ├── UserService/         # User management (SQL Server + EF Core)
│   ├── OrderService/        # Order management (PostgreSQL + EF Core)
│   └── ProductService/      # Product catalog (MongoDB)
├── Shared/
│   ├── DTOs/                # Common DTOs and response models
│   ├── Auth/                # JWT token service
│   └── Messaging/           # Event publishing (RabbitMQ)
└── docker-compose.yml       # Complete infrastructure setup
```

## Features

### Architecture
- ✅ Clean Architecture (Presentation, Application, Domain, Infrastructure)
- ✅ Repository Pattern
- ✅ Dependency Injection
- ✅ CQRS pattern ready
- ✅ Error handling and logging

### Authentication & Security
- ✅ JWT Bearer token authentication
- ✅ Password hashing with BCrypt
- ✅ Role-based authorization (RBAC)
- ✅ API Gateway authentication/authorization

### Database Support
- ✅ SQL Server (UserService)
- ✅ PostgreSQL (OrderService)
- ✅ MongoDB (ProductService)
- ✅ Entity Framework Core with migrations
- ✅ Database initialization scripts

### Messaging
- ✅ RabbitMQ message broker
- ✅ Event-driven architecture ready
- ✅ Event publishing and subscription patterns
- ✅ Domain events

### API Gateway
- ✅ Ocelot API Gateway
- ✅ Request routing and transformation
- ✅ Rate limiting ready
- ✅ Service discovery

### DevOps
- ✅ Docker and Docker Compose
- ✅ Multi-container orchestration
- ✅ Environment configuration
- ✅ Health checks ready

### Monitoring
- ✅ Prometheus integration ready
- ✅ Grafana dashboard ready
- ✅ Structured logging

## Getting Started

### Prerequisites
- .NET 8 SDK or later
- Docker and Docker Compose
- SQL Server (or use Docker)
- PostgreSQL (or use Docker)
- MongoDB (or use Docker)
- RabbitMQ (or use Docker)

### Quick Start with Docker

1. Clone the repository:
```bash
cd saas-platform
```

2. Build the solution:
```bash
dotnet build SaasPlatform.sln
```

3. Start all services with Docker Compose:
```bash
docker-compose up -d
```

4. Access the services:
- API Gateway: `http://localhost:5000`
- User Service: `http://localhost:5001`
- Order Service: `http://localhost:5002`
- Product Service: `http://localhost:5003`
- RabbitMQ Admin: `http://localhost:15672` (guest/guest)
- Grafana: `http://localhost:3000` (admin/admin)

### Running Services Locally

#### User Service:
```bash
cd Services/UserService
dotnet restore
dotnet run
```

#### Order Service:
```bash
cd Services/OrderService
dotnet restore
dotnet run
```

#### Product Service:
```bash
cd Services/ProductService
dotnet restore
dotnet run
```

#### API Gateway:
```bash
cd ApiGateway
dotnet restore
dotnet run
```

## API Endpoints

### User Service
- `GET /api/users` - Get all users (requires Auth)
- `GET /api/users/{id}` - Get user by ID (requires Auth)
- `GET /api/users/by-username/{username}` - Get user by username (requires Auth)
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user (requires Auth)
- `DELETE /api/users/{id}` - Delete user (requires Auth)

### Order Service
- `GET /api/orders` - Get all orders (requires Auth)
- `GET /api/orders/{id}` - Get order by ID (requires Auth)
- `GET /api/orders/user/{userId}` - Get orders by user (requires Auth)
- `POST /api/orders` - Create new order (requires Auth)
- `PUT /api/orders/{id}` - Update order (requires Auth)
- `DELETE /api/orders/{id}` - Delete order (Admin only)

### Product Service
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/by-sku/{sku}` - Get product by SKU
- `GET /api/products/category/{category}` - Get products by category
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

## Authentication

### Getting a JWT Token

Sample request to User Service:
```bash
curl -X POST http://localhost:5001/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "email": "john@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "SecurePassword123!"
  }'
```

### Using the Token

Add the token to subsequent requests:
```bash
curl -X GET http://localhost:5000/api/users \
  -H "Authorization: Bearer <your-jwt-token>"
```

## Configuration

### Environment Variables

Each service can be configured via `appsettings.json` or environment variables:

```env
# User Service
ConnectionStrings__DefaultConnection=Server=sqlserver;Database=UserServiceDb;User Id=sa;Password=YourPassword123!;
Jwt__Secret=your-secret-key-should-be-very-long-and-secure-at-least-32-characters

# Order Service
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=OrderServiceDb;Username=postgres;Password=YourPassword123!;

# Product Service
MongoDbSettings__ConnectionString=mongodb://mongo:27017
MongoDbSettings__DatabaseName=ProductServiceDb
```

## Database Migrations

### User Service (SQL Server)
```bash
cd Services/UserService
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Order Service (PostgreSQL)
```bash
cd Services/OrderService
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Project Structure Details

### Clean Architecture Layers

Each microservice follows these layers:

1. **Presentation Layer** (`Presentation/`)
   - Controllers
   - Request/Response models
   - API endpoints

2. **Application Layer** (`Application/`)
   - Business logic
   - Services
   - DTOs
   - Use cases

3. **Domain Layer** (`Domain/`)
   - Entities
   - Interfaces
   - Business rules

4. **Infrastructure Layer** (`Infrastructure/`)
   - Database contexts
   - Repository implementations
   - External service integrations
   - Data access logic

## Messaging

### RabbitMQ Events

Pre-defined events:
- `UserCreatedEvent` - When a new user is created
- `UserUpdatedEvent` - When a user is updated
- `UserDeletedEvent` - When a user is deleted
- `OrderCreatedEvent` - When a new order is created
- `OrderShippedEvent` - When an order is shipped
- `ProductCreatedEvent` - When a new product is created
- `ProductUpdatedEvent` - When a product is updated

To implement event handling, inject `IEventPublisher` or `IEventSubscriber` in your services.

## Security Best Practices

1. **Secrets Management**
   - Store sensitive data in environment variables or secret manager
   - Never commit secrets to version control
   - Use Azure Key Vault in production

2. **Database Security**
   - Use strong passwords
   - Implement connection pooling
   - Enable encryption at rest

3. **API Security**
   - Implement HTTPS/TLS
   - Use JWT for authentication
   - Implement rate limiting
   - Input validation and sanitization

4. **CORS**
   - Configure CORS for allowed origins
   - Restrict to specific domains in production

## Monitoring & Logging

- **Structured Logging**: Using ILogger for all services
- **Prometheus**: Metrics collection (ready for integration)
- **Grafana**: Visualization dashboard (pre-configured)
- **Health Checks**: Implement health check endpoints for Docker

## Testing

Example using xUnit and Moq (to be added):
```bash
dotnet test
```

## Contributing

1. Create a feature branch
2. Make your changes
3. Commit with clear messages
4. Push to the branch
5. Create a Pull Request

## Performance Considerations

- Connection pooling for databases
- Caching strategy (Redis ready)
- Async/await throughout
- Lazy loading where appropriate
- Query optimization for large datasets

## Troubleshooting

### Common Issues

1. **Container won't start**
   - Check Docker is running
   - Check port availability
   - Review container logs: `docker logs <container-name>`

2. **Database connection fails**
   - Verify connection strings
   - Check database containers are running
   - Check default passwords

3. **Authentication failures**
   - Verify JWT secret matches across services
   - Check token hasn't expired
   - Verify Authorization header format

## License

Proprietary - All rights reserved

## Support

For issues and questions, please contact the development team.

---

Built with ❤️ using .NET 8 and Clean Architecture principles
