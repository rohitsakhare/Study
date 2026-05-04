# Microservices Solution

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker)](https://www.docker.com/)
[![Kubernetes](https://img.shields.io/badge/Kubernetes-326CE5?style=flat&logo=kubernetes)](https://kubernetes.io/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A production-ready microservices architecture built with .NET 10.0, demonstrating modern distributed systems patterns, containerization, and cloud-native deployment.

## ✨ Features

- 🏗️ **Microservices Architecture** - Decoupled, independently deployable services
- 🚪 **API Gateway** - Centralized routing with Ocelot
- 🐳 **Containerization** - Docker support for all services
- ☸️ **Orchestration** - Kubernetes manifests with auto-scaling
- 📡 **RESTful APIs** - Clean, documented endpoints
- 🔄 **Service Discovery** - Inter-service communication
- 📊 **Monitoring Ready** - Health checks and logging
- 🧪 **Testing Support** - HTTP files for API testing

## 🏛️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    External Clients                         │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                 API Gateway (Ocelot)                        │
│                 Port: 8000 (Docker) / 30007 (K8s)           │
└─────────────────────┬───────────────────────────────────────┘
                      │
           ┌──────────┼──────────┐
           │          │          │
           ▼          ▼          ▼
┌─────────────────┐ ┌────────────┐ ┌─────────────────┐
│  User Service   │ │Product Svc │ │  Order Service  │
│  Port: 5000     │ │ Port: 5001 │ │  Port: 5002     │
│                 │ │            │ │                 │
│ • User Mgmt     │ │ • Catalog  │ │ • Order Proc.   │
│ • Auth Ready    │ │ • Inventory│ │ • Fulfillment  │
└─────────────────┘ └────────────┘ └─────────────────┘
```

## 🚀 Quick Start

### Docker Compose (Recommended)
```bash
git clone <repository-url>
cd MicroservicesSolution
docker-compose up --build
```

**Access:** `http://localhost:8000`

### Kubernetes
```bash
minikube start
kubectl apply -f k8s/
```

**Access:** `http://$(minikube ip):30007`

## 📋 Table of Contents

- [Services Overview](#services-overview)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
- [API Reference](#api-reference)
- [Development](#development)
- [Deployment](#deployment)
- [Configuration](#configuration)
- [Testing](#testing)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

## 🔧 Services Overview

### API Gateway
- **Framework**: ASP.NET Core + Ocelot
- **Purpose**: Request routing, load balancing, authentication
- **Configuration**: `ocelot.json`
- **Port**: 8000 (Docker), 30007 (K8s)

### User Service
- **Framework**: ASP.NET Core Web API
- **Purpose**: User management and authentication
- **Database**: In-memory (demo)
- **Port**: 5000

### Product Service
- **Framework**: ASP.NET Core Web API
- **Purpose**: Product catalog and inventory
- **Database**: PostgreSQL with Entity Framework Core
- **Port**: 5001

### Order Service
- **Framework**: ASP.NET Core Web API
- **Purpose**: Order processing and management
- **Database**: In-memory (demo)
- **Port**: 5002

## 📋 Prerequisites

- **.NET 10.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/10.0)
- **PostgreSQL** - [Download](https://www.postgresql.org/download/) or use Docker
- **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
- **kubectl** (Kubernetes) - [Install](https://kubernetes.io/docs/tasks/tools/)
- **Minikube** (Local K8s) - [Install](https://minikube.sigs.k8s.io/docs/start/)

## 🛠️ Installation

1. **Clone Repository**
   ```bash
   git clone <repository-url>
   cd MicroservicesSolution
   ```

2. **Install Dependencies** (Local Development)
   ```bash
   # Restore all services
   dotnet restore
   ```

3. **Verify Setup**
   ```bash
   dotnet --version  # Should show 10.0.x
   docker --version  # Should show version
   ```

## 🎯 Usage

### Local Development

Run services individually for development and debugging:

```bash
# Terminal 1 - User Service
cd UserService && dotnet run

# Terminal 2 - Product Service
cd ProductService && dotnet run

# Terminal 3 - Order Service
cd OrderService && dotnet run

# Terminal 4 - API Gateway
cd ApiGateway && dotnet run
```

**Endpoints:**
- API Gateway: `http://localhost:8000`
- User Service: `http://localhost:5000`
- Product Service: `http://localhost:5001`
- Order Service: `http://localhost:5002`

### Docker Development

#### Build All Services
```bash
docker-compose build
```

#### Run All Services
```bash
docker-compose up
```

#### Run in Background
```bash
docker-compose up -d
```

#### View Logs
```bash
docker-compose logs -f [service-name]
```

#### Stop Services
```bash
docker-compose down
```

#### Build Individual Images
```bash
docker build -t userservice ./UserService
docker build -t productservice ./ProductService
docker build -t orderservice ./OrderService
docker build -t apigateway ./ApiGateway
```

### Kubernetes Deployment

#### Start Local Cluster
```bash
minikube start
minikube addons enable ingress  # Optional
```

#### Deploy Services
```bash
# Deploy all services including PostgreSQL database
kubectl apply -f k8s/microservices.yaml

# Or deploy PostgreSQL separately first (optional)
# kubectl apply -f k8s/postgresql.yaml
# kubectl apply -f k8s/microservices.yaml
```

#### Check Deployment
```bash
kubectl get pods -n microservices
kubectl get services -n microservices
kubectl get hpa -n microservices
```

#### Access Application
```bash
# Get cluster IP
minikube ip

# Access API Gateway
curl http://$(minikube ip):30007/users
```

#### Scale Services
```bash
# Manual scaling
kubectl scale deployment userservice --replicas=3 -n microservices

# View HPA status
kubectl get hpa -n microservices
```

#### Cleanup
```bash
kubectl delete -f k8s/
minikube stop
```

## 📚 API Reference

All APIs are accessible through the API Gateway.

### Base URLs
- **Docker**: `http://localhost:8000`
- **Kubernetes**: `http://<minikube-ip>:30007`

### Endpoints

| Method | Endpoint | Description | Service | Status |
|--------|----------|-------------|---------|--------|
| GET | `/users` | Retrieve all users | User | ✅ |
| GET | `/products` | Retrieve all products (with filtering) | Product | ✅ |
| GET | `/products/{id}` | Get product by ID | Product | ✅ |
| POST | `/products` | Create new product | Product | ✅ |
| PUT | `/products/{id}` | Update product | Product | ✅ |
| DELETE | `/products/{id}` | Delete product | Product | ✅ |
| GET | `/products/categories` | Get product categories | Product | ✅ |
| GET | `/orders` | Retrieve all orders | Order | ✅ |

### Product API Features

The Product Service now supports:
- **Full CRUD operations** for product management
- **Advanced filtering** by category, price range, active status
- **Search functionality** in product names and descriptions
- **PostgreSQL persistence** with automatic migrations and dynamic seeding
- **Automatic initialization** on startup (creates tables and seeds sample data)
- **Data validation** and error handling
- **CORS support** for cross-origin requests

### Example Requests

#### Get All Products
```bash
curl "http://localhost:8000/products"
```

#### Get Products with Filters
```bash
# Filter by category, price range, and active status
curl "http://localhost:8000/products?category=Electronics&minPrice=100&maxPrice=1500&isActive=true"

# Search by name/description
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
       "name": "Wireless Headphones",
       "description": "Noise-cancelling wireless headphones",
       "price": 199.99,
       "stockQuantity": 75,
       "category": "Electronics"
     }'
```

#### Update Product (Full Update)
```bash
curl -X PUT "http://localhost:8000/products/1" \
     -H "Content-Type: application/json" \
     -d '{
       "name": "Premium Wireless Headphones",
       "description": "Advanced noise-cancelling wireless headphones",
       "price": 249.99,
       "stockQuantity": 60,
       "category": "Electronics"
     }'
```

#### Update Product (Partial Update)
```bash
curl -X PUT "http://localhost:8000/products/1" \
     -H "Content-Type: application/json" \
     -d '{"price": 249.99, "stockQuantity": 60}'
```

#### Delete Product
```bash
curl -X DELETE "http://localhost:8000/products/1"
```

#### Get Product Categories
```bash
curl "http://localhost:8000/products/categories"
```

### Response Examples

#### GET /users
```json
[
  {
    "id": 1,
    "name": "Alice"
  },
  {
    "id": 2,
    "name": "Bob"
  }
]
```

#### GET /products (List with Filtering)
```json
[
  {
    "id": 1,
    "name": "Laptop",
    "description": "High-performance laptop for professionals",
    "price": 1299.99,
    "stockQuantity": 50,
    "category": "Electronics",
    "isActive": true,
    "createdAt": "2026-02-20T00:00:00Z",
    "updatedAt": "2026-02-20T00:00:00Z"
  },
  {
    "id": 2,
    "name": "Phone",
    "description": "Latest smartphone with advanced features",
    "price": 899.99,
    "stockQuantity": 100,
    "category": "Electronics",
    "isActive": true,
    "createdAt": "2026-02-20T00:00:00Z",
    "updatedAt": "2026-02-20T00:00:00Z"
  }
]
```

#### GET /products/{id} (Single Product)
```json
{
  "id": 1,
  "name": "Laptop",
  "description": "High-performance laptop for professionals",
  "price": 1299.99,
  "stockQuantity": 50,
  "category": "Electronics",
  "isActive": true,
  "createdAt": "2026-02-20T00:00:00Z",
  "updatedAt": "2026-02-20T00:00:00Z"
}
```

#### POST /products (Create)
```json
{
  "id": 4,
  "name": "Wireless Keyboard",
  "description": "Mechanical wireless keyboard",
  "price": 89.99,
  "stockQuantity": 150,
  "category": "Accessories",
  "isActive": true,
  "createdAt": "2026-02-20T10:30:45Z",
  "updatedAt": "2026-02-20T10:30:45Z"
}
```

#### GET /products/categories
```json
[
  "Electronics",
  "Accessories",
  "Computing"
]
```

#### GET /orders
```json
[
  {
    "id": 1,
    "product": "Laptop",
    "user": "Alice"
  },
  {
    "id": 2,
    "product": "Phone",
    "user": "Bob"
  }
]
```

## 📚 Service Documentation

For detailed information about each service, refer to their individual README files:

- **[API Gateway README](./ApiGateway/README.md)** - Routing configuration, endpoint management, and platform integration
- **[Product Service README](./ProductService/README.md)** - CRUD operations, PostgreSQL setup, automatic migrations, and data seeding
- **[User Service README](./UserService/README.md)** - User management and authentication patterns
- **[Order Service README](./OrderService/README.md)** - Order processing and fulfillment workflow

## 💻 Development

### Project Structure
```
MicroservicesSolution/
├── ApiGateway/                          # API Gateway Service (Ocelot)
│   ├── ocelot.json                      # Route Configuration
│   ├── Program.cs                       # ASP.NET Core Startup
│   ├── ApiGateway.csproj                # Project File
│   ├── ApiGateway.http                  # API Test Requests
│   ├── README.md                        # Service Documentation
│   ├── Dockerfile                       # Container Definition
│   ├── appsettings.json                 # Configuration
│   ├── appsettings.Development.json     # Development Overrides
│   └── Properties/
│       └── launchSettings.json          # VS Launch Settings
│
├── UserService/                         # User Management Microservice
│   ├── Controllers/
│   │   └── UsersController.cs           # User API Endpoints
│   ├── Program.cs                       # ASP.NET Core Startup
│   ├── UserService.csproj               # Project File
│   ├── UserService.http                 # API Test Requests
│   ├── README.md                        # Service Documentation
│   ├── Dockerfile                       # Container Definition
│   ├── appsettings.json                 # Configuration
│   ├── appsettings.Development.json     # Development Overrides
│   └── Properties/
│       └── launchSettings.json          # VS Launch Settings
│
├── ProductService/                      # Product Catalog Microservice (PostgreSQL)
│   ├── Controllers/
│   │   └── ProductsController.cs        # CRUD Product Endpoints
│   ├── Models/
│   │   └── Product.cs                   # Product Entity
│   ├── Data/
│   │   └── ApplicationDbContext.cs      # EF Core Context with Seeding
│   ├── DTOs/
│   │   └── ProductDtos.cs               # CreateProductDto, UpdateProductDto, ProductDto
│   ├── Migrations/                      # Entity Framework Migrations
│   ├── Program.cs                       # ASP.NET Core Startup + DB Initialization
│   ├── ProductService.csproj            # Project File
│   ├── ProductService.http              # API Test Requests (CRUD examples)
│   ├── README.md                        # Service Documentation
│   ├── Dockerfile                       # Multi-stage Container Build
│   ├── appsettings.json                 # Configuration with Connection String
│   ├── appsettings.Development.json     # Development Overrides
│   └── Properties/
│       └── launchSettings.json          # VS Launch Settings
│
├── OrderService/                        # Order Processing Microservice
│   ├── Controllers/
│   │   └── OrdersController.cs          # Order API Endpoints
│   ├── Program.cs                       # ASP.NET Core Startup
│   ├── OrderService.csproj              # Project File
│   ├── OrderService.http                # API Test Requests
│   ├── README.md                        # Service Documentation
│   ├── Dockerfile                       # Container Definition
│   ├── appsettings.json                 # Configuration
│   ├── appsettings.Development.json     # Development Overrides
│   └── Properties/
│       └── launchSettings.json          # VS Launch Settings
│
├── k8s/                                 # Kubernetes Manifests
│   └── microservices.yaml               # Complete Deployment & Services
│
├── docker-compose.yml                   # Docker Compose Orchestration
│   │                                    # - PostgreSQL Service
│   │                                    # - All Microservices
│   │                                    # - Health Checks & Dependencies
│   │
├── MicroservicesSolution.slnx           # Solution File (Visual Studio)
├── .gitignore                           # Git Ignore Rules
└── README.md                            # This File
```

### Key Directories Explained

- **ApiGateway**: Ocelot-based API gateway routing all client requests to backend services
- **ProductService**: PostgreSQL-backed service with EF Core migrations, automatic seeding, and complete CRUD operations
- **UserService/OrderService**: In-memory demo services showing microservice patterns
- **k8s/**: Kubernetes deployment manifests for production-like orchestration
- **docker-compose.yml**: Local development with all services in Docker containers

### Development Workflow

1. **Choose Development Mode**
   - Local: Run services individually with `dotnet run`
   - Docker: Use `docker-compose up` for isolated environment with PostgreSQL
   - Kubernetes: Use `kubectl apply` for production-like setup

2. **Setup Prerequisites**
   - Ensure PostgreSQL is running (required for ProductService)
   - Default local connection: `Host=localhost;Port=5432;Database=ProductDb`
   - Docker connection: `Host=postgres;Port=5432;Database=ProductDb`

3. **Make Changes**
   - Edit code in your preferred IDE
   - Services auto-reload in development mode
   - Database migrations and seeding happen automatically on startup

4. **Test Changes**
   - Use `.http` files for API testing in VS Code (install REST Client extension)
   - Check logs with `docker-compose logs` or `kubectl logs`
   - ProductService seedsample data on first run (Laptop, Phone, Headphones)

5. **Build and Deploy**
   ```bash
   # Build for production
   dotnet publish --configuration Release

   # Build Docker images
   docker-compose build

   # Deploy to Kubernetes
   kubectl apply -f k8s/
   ```

### Code Quality

- Follow .NET coding standards
- Use meaningful variable names
- Add XML documentation comments
- Write unit tests for business logic
- Keep services loosely coupled

## 🚀 Deployment

### Docker Compose (Development/Testing)
```bash
# Development
docker-compose -f docker-compose.yml up --build

# Production
docker-compose -f docker-compose.prod.yml up -d
```

### Kubernetes (Production)
```bash
# Deploy
kubectl apply -f k8s/

# Check health
kubectl get pods -n microservices
kubectl get services -n microservices

# Monitor
kubectl logs -f deployment/apigateway -n microservices
```

### CI/CD Pipeline
```yaml
# Example GitHub Actions workflow
name: Deploy to Kubernetes
on: [push]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v2
    - name: Build Docker images
      run: docker-compose build
    - name: Deploy to K8s
      run: kubectl apply -f k8s/
```

## 🗄️ Database

### ProductService Database Setup

**Automatic Initialization:**
- Entity Framework migrations are applied automatically on startup
- Initial products are seeded if the Products table is empty
- Table and column names are configured to lowercase for PostgreSQL
- No manual migration commands required for first-time setup

**Connection Strings:**
- Local: `Host=localhost;Port=5432;Database=ProductDb;Username=postgres;Password=password`
- Docker: `Host=postgres;Port=5432;Database=ProductDb;Username=postgres;Password=password`

**Seed Data** (auto-populated on first run):
- Laptop (Electronics, $1,299.99, 50 units)
- Phone (Electronics, $899.99, 100 units)
- Headphones (Electronics, $199.99, 75 units)

For more details, see [ProductService README](./ProductService/README.md)

## ⚙️ Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|----------|
| `ASPNETCORE_ENVIRONMENT` | Environment | `Development` |
| `ASPNETCORE_URLS` | Server URLs | `http://+:8080` |
| `ConnectionStrings__DefaultConnection` | Database connection string (ProductService) | From appsettings.json |

### App Settings

Each service has `appsettings.json` and `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### ProductService-Specific Configuration

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ProductDb;Username=postgres;Password=password"
  }
}
```

**Docker Environment Override**:
```yaml
environment:
  ConnectionStrings__DefaultConnection: "Host=postgres;Port=5432;Database=ProductDb;Username=postgres;Password=password"
```

**Automatic Features**:
- Migrations applied at startup via `dbContext.Database.Migrate()`
- Initial products seeded via `dbContext.SeedInitialProducts()`
- Lowercase table/column configuration for PostgreSQL compatibility

### Ocelot Configuration

API Gateway routing is configured in `ocelot.json`:

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/users",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        { "Host": "userservice", "Port": 8080 }
      ],
      "UpstreamPathTemplate": "/users",
      "UpstreamHttpMethod": ["GET"]
    }
  ]
}
```

## 🧪 Testing

### API Testing

Each service includes `.http` files for testing:

1. Install "REST Client" extension in VS Code
2. Open `.http` file
3. Click "Send Request" above each request

### Unit Testing

```bash
# Run tests for all services
dotnet test

# Run tests for specific service
cd UserService && dotnet test
```

### Integration Testing

```bash
# Test with Docker Compose
docker-compose -f docker-compose.test.yml up --abort-on-container-exit

# Test with Kubernetes
kubectl apply -f k8s/test/
```

### Load Testing

```bash
# Use tools like Apache Bench or k6
ab -n 1000 -c 10 http://localhost:8000/users
```

## 🔧 Troubleshooting

### Common Issues

#### Port Conflicts
```bash
# Check port usage
netstat -ano | findstr :5000

# Kill process using port
taskkill /PID <PID> /F
```

#### Docker Issues
```bash
# Clear Docker cache
docker system prune -a

# Check container logs
docker-compose logs [service-name]

# Rebuild without cache
docker-compose build --no-cache
```

#### Kubernetes Issues
```bash
# Check pod status
kubectl get pods -n microservices
kubectl describe pod <pod-name> -n microservices

# Check service endpoints
kubectl get endpoints -n microservices

# Restart deployment
kubectl rollout restart deployment/apigateway -n microservices
```

#### Build Failures
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build

# Check .NET version
dotnet --version
```

### Debug Mode

Run services in debug mode:

```bash
# With debugger attached
dotnet run --launch-profile "Debug"

# With detailed logging
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

### Health Checks

```bash
# Docker
curl http://localhost:8000/health

# Kubernetes
curl http://$(minikube ip):30007/health
```

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/AmazingFeature`)
3. **Commit** your changes (`git commit -m 'Add some AmazingFeature'`)
4. **Push** to the branch (`git push origin feature/AmazingFeature`)
5. **Open** a Pull Request

### Guidelines

- 📝 **Documentation**: Update README for new features
- 🧪 **Testing**: Add tests for new functionality
- 🎨 **Style**: Follow .NET coding conventions
- 🔄 **CI/CD**: Ensure builds pass
- 📦 **Dependencies**: Keep packages updated

### Development Setup

```bash
# Clone and setup
git clone <your-fork-url>
cd MicroservicesSolution
dotnet restore

# Run tests
dotnet test

# Start development environment
docker-compose up
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [.NET Team](https://dotnet.microsoft.com/) for the amazing framework
- [Ocelot](https://github.com/ThreeMammals/Ocelot) for API Gateway
- [Docker](https://www.docker.com/) for containerization
- [Kubernetes](https://kubernetes.io/) for orchestration

## 📞 Support

- 📧 **Email**: [your-email@example.com]
- 💬 **Issues**: [GitHub Issues](https://github.com/your-repo/issues)
- 📖 **Documentation**: [Wiki](https://github.com/your-repo/wiki)

---

⭐ **Star this repo** if you find it helpful!