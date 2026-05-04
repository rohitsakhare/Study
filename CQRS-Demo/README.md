# CQRS-Demo

A sample .NET 9 CQRS demo with separate write and read services, Kafka event streaming, and SQL Server databases.

## Architecture

- `ProductService`: write-side API that accepts product creation requests and stores data in a write database.
- `ProductReadService`: read-side API that reads from a dedicated read database and serves product queries.
- `Kafka`: used as an event bus to propagate `ProductCreatedEvent` messages from the write side to the read side.
- `Outbox Pattern`: `ProductService` saves events to an outbox table and a background worker publishes them to Kafka.
- `ProductConsumer`: `ProductReadService` consumes Kafka events and populates a read model view.

## Services

### ProductService

- URL: `http://localhost:5001`
- Endpoint: `POST /api/products`
- Request body example:
  ```json
  {
    "name": "Sample Product",
    "price": 19.99
  }
  ```
- Uses `MediatR`, Entity Framework Core, Dapper, and Kafka producer.

### ProductReadService

- URL: `http://localhost:5002`
- Endpoint: `GET /api/products`
- Returns product data from the `ProductView` read model.
- Uses `MediatR`, Dapper, and Kafka consumer.

## Docker Compose Setup

The repository includes a `docker-compose.yml` file that starts:

- `zookeeper`
- `kafka`
- `kafka-ui` (Kafka management UI on port `8080`)
- `write-db` (SQL Server for write-side data on port `1433`)
- `read-db` (SQL Server for read-side data on port `1434`)
- `db-init` (initializes the write/read databases and schema)
- `productservice` (write service)
- `productreadservice` (read service)

## Database Initialization

SQL scripts are located in `scripts/`:

- `write-db-init.sql`: creates `ProductWriteDb`, `Products`, and `OutboxMessages`.
- `read-db-init.sql`: creates `ProductReadDb` and `ProductView`.

## Getting Started

### Prerequisites

- Docker Desktop
- .NET 9 SDK (if you want to build/run locally outside Docker)

### Run with Docker Compose

From the repository root:

```powershell
docker-compose up --build
```

### Access APIs

- Product write API: `http://localhost:5001/api/products`
- Product read API: `http://localhost:5002/api/products`

### Swagger / API docs

Each service exposes Swagger UI:

- `http://localhost:5001/swagger`
- `http://localhost:5002/swagger`

### Kafka UI

- `http://localhost:8080`

## Notes

- `ProductService` publishes `ProductCreatedEvent` objects to Kafka topic `product-events`.
- `ProductReadService` listens to the same topic and updates the `ProductView` table.
- The write service uses an outbox worker to publish unprocessed messages reliably.

## Project Structure

- `ProductService/`
  - `Controllers/ProductController.cs`
  - `Application/Commands/CreateProductCommand.cs`
  - `Application/Handlers/CreateProductHandler.cs`
  - `Domain/` - domain event and entity models
  - `Infrastructure/Outbox/OutboxWorker.cs`
  - `Infrastructure/Kafka/KafkaProducer.cs`
  - `Infrastructure/Data/WriteDbContext.cs`

- `ProductReadService/`
  - `Controllers/ProductController.cs`
  - `Application/Queries/GetProductsQuery.cs`
  - `Application/Handlers/GetProductsHandler.cs`
  - `Infrastructure/Kafka/ProductConsumer.cs`
  - `Infrastructure/Data/DbConnectionFactory.cs`

## Local development

If running services locally instead of Docker, update connection strings and Kafka settings in `appsettings.json` or environment variables.

## License

This repository is provided as a sample demonstration and does not include a license file.
