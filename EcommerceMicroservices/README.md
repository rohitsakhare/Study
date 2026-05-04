# E-Commerce Microservices

A distributed microservices architecture for an e-commerce platform built with .NET 9, PostgreSQL, Kafka, and Docker.

## Project Overview

This solution demonstrates a modern microservices architecture with the following components:

- **OrderService**: Manages order creation and processing
- **InventoryService**: Handles product inventory management
- **PaymentService**: Processes payment transactions
- **NotificationService**: Sends notifications for various events
- **Shared**: Common models and event bus implementation

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                  Microservices                          │
├──────────────────┬──────────────────┬──────────────────┤
│  OrderService    │ InventoryService │  PaymentService  │
│  (Port 5223)     │   (Port 5282)    │    (Port 5024)   │
└────────┬─────────┴────────┬─────────┴────────┬─────────┘
         │                  │                  │
         └──────────────────┼──────────────────┘
                            │
                    ┌───────▼────────┐
                    │  Event Bus     │
                    │  (Kafka)       │
                    │  Port 9092     │
                    └────────────────┘
                            │
         ┌──────────────────┴──────────────────┐
         │                                     │
    ┌────▼────┐  ┌───────────┐  ┌────────────┐│
    │PostgreSQL│  │PostgreSQL │  │ PostgreSQL ││
    │OrderDb   │  │InventoryDb│  │ PaymentDb ││
    │5432     │  │  5433     │  │  5434     ││
    └─────────┘  └───────────┘  └───────────┘│
                                              │
                           NotificationService
                              (Port 5186)
```

## Prerequisites

- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Docker** and **Docker Compose** - [Download](https://www.docker.com/products/docker-desktop)
- **PostgreSQL** (optional if using Docker)
- **Kafka** (optional if using Docker)

## Getting Started

### Quick Start with Docker (Recommended)

Run the entire solution with Docker Compose:

```bash
cd d:\R_Data\Study\microservices\ECommerceMicroservices
docker-compose up -d
```

This will:
1. Start all infrastructure services (Kafka, Zookeeper, PostgreSQL)
2. Build and start all microservices in containers
3. Expose all services on their respective ports

All services will be available:
- **OrderService**: http://localhost:5223
- **InventoryService**: http://localhost:5282
- **PaymentService**: http://localhost:5024
- **NotificationService**: http://localhost:5186

### Local Development (Without Docker)

#### 1. Clone the Repository

```bash
cd d:\R_Data\Study\microservices\ECommerceMicroservices
```

#### 2. Start Infrastructure Services

The `docker-compose.yml` file can be used to start only infrastructure services:

```bash
docker-compose up -d zookeeper kafka postgres-order postgres-inventory postgres-payment
```

This starts:
- **Zookeeper**: Port 2181
- **Kafka**: Port 9092
- **PostgreSQL (Order DB)**: Port 5432
- **PostgreSQL (Inventory DB)**: Port 5433
- **PostgreSQL (Payment DB)**: Port 5434

#### 3. Build the Solution

```bash
dotnet build
```

#### 4. Run Microservices Locally

Run each microservice in a separate terminal:

**OrderService:**
```bash
cd OrderService
dotnet run
```
Listens on: `http://localhost:5223`

**InventoryService:**
```bash
cd InventoryService
dotnet run
```
Listens on: `http://localhost:5282`

**PaymentService:**
```bash
cd PaymentService
dotnet run
```
Listens on: `http://localhost:5024`

**NotificationService:**
```bash
cd NotificationService
dotnet run
```
Listens on: `http://localhost:5186`

## Project Structure

```
ECommerceMicroservices/
├── OrderService/              # Order management microservice
│   ├── Controllers/
│   │   └── OrderController.cs
│   ├── Data/
│   │   └── OrderDbContext.cs
│   ├── Models/
│   │   └── Order.cs
│   ├── appsettings.json
│   └── Program.cs
├── InventoryService/          # Inventory management microservice
│   ├── Controllers/
│   │   └── InventoryController.cs
│   ├── Data/
│   │   └── InventoryDbContext.cs
│   ├── Models/
│   │   └── InventoryItem.cs
│   ├── appsettings.json
│   └── Program.cs
├── PaymentService/            # Payment processing microservice
│   ├── Controllers/
│   │   └── PaymentController.cs
│   ├── Data/
│   │   └── PaymentDbContext.cs
│   ├── Models/
│   │   └── Payment.cs
│   ├── appsettings.json
│   └── Program.cs
├── NotificationService/       # Notification microservice
│   ├── Controllers/
│   │   └── NotificationController.cs
│   ├── appsettings.json
│   └── Program.cs
├── Shared/                    # Shared libraries and event bus
│   ├── EventBus/
│   │   ├── KafkaConsumer.cs
│   │   └── KafkaProducer.cs
│   ├── Models/
│   │   └── Events.cs
│   └── Shared.csproj
├── docker-compose.yml         # Infrastructure services configuration
└── ECommerceMicroservices.sln # Solution file
```

## Configuration

### Database Connection Strings

Edit `appsettings.json` in each service to configure database connections:

```json
{
  "ConnectionStrings": {
    "OrderDb": "Host=localhost;Port=5432;Database=OrderDb;Username=postgres;Password=password"
  }
}
```

### Kafka Configuration

Services connect to Kafka at `localhost:9092` for event-based communication. Configure Kafka bootstrap servers in `appsettings.json` if needed.

## API Endpoints

### Order Service
- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create new order

### Inventory Service
- `GET /api/inventory` - Get all inventory items
- `GET /api/inventory/{id}` - Get inventory item by ID
- `POST /api/inventory` - Create new item
- `PUT /api/inventory/{id}` - Update inventory

### Payment Service
- `GET /api/payments` - Get all payments
- `GET /api/payments/{id}` - Get payment by ID
- `POST /api/payments` - Process payment

### Notification Service
- `GET /api/notifications` - Get notifications
- `POST /api/notifications` - Send notification

## Technologies Used

- **.NET 9** - Application framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM for data access
- **PostgreSQL** - Relational database (Alpine images for optimization)
- **Kafka** - Event streaming platform
- **Docker & Docker Compose** - Containerization
- **Zookeeper** - Kafka coordination

## Stopping the Services

### Docker Compose
Stop all services and clean up containers:

```bash
docker-compose down
```

To remove volumes (databases) as well:
```bash
docker-compose down -v
```

### Local Development
Press `Ctrl+C` in each terminal running a microservice.

Then stop infrastructure services:
```bash
docker-compose down
```

## Development

### Database Migrations

To apply Entity Framework Core migrations:

```bash
cd [ServiceName]
dotnet ef database update
```

### Adding Dependencies

```bash
dotnet add [ServiceName] package [NugetPackage]
```

## Docker Configuration

### Image Optimization

The solution uses lightweight Docker images for optimal performance:

- **PostgreSQL**: `postgres:15-alpine` - Alpine-based PostgreSQL for reduced image size
- **Microservices**: Multi-stage Dockerfile with:
  - **Build stage**: `mcr.microsoft.com/dotnet/sdk:9.0` - Full SDK for compilation
  - **Runtime stage**: `mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled` - Minimal .NET runtime (Chiseled image)

The Chiseled runtime images are significantly smaller than standard images, reducing:
- Image size by ~70%
- Memory footprint
- Attack surface and security vulnerabilities
- Build and deployment time

### Dockerfile Structure

Each microservice uses a two-stage Docker build:

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "ECommerceMicroservices.sln"
RUN dotnet build "ECommerceMicroservices.sln" -c Release
RUN dotnet publish "ServiceName/ServiceName.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ServiceName.dll"]
```

### Docker Compose Features

- **Custom Network**: All services connected via `ecommerce` bridge network for internal communication
- **Health Checks**: PostgreSQL services include health checks for dependency management
- **Service Dependencies**: Microservices wait for database health checks before starting
- **Environment Variables**: Production environment configured for container deployment

## Troubleshooting

### Port Already in Use
If a port is already in use, update the port mappings in `docker-compose.yml` or `launchSettings.json`.

### Database Connection Errors
Ensure PostgreSQL services are running:
```bash
docker-compose ps
```

### Kafka Connection Issues
Verify Kafka is running on port 9092:
```bash
docker logs kafka
```

## License

This project is provided for educational purposes.

## Contributing

Contributions are welcome. Please follow the existing code structure and conventions.
