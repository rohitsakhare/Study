# Product Service

`ProductService` is an ASP.NET Core (.NET 9) microservice responsible for product CRUD operations and publishing Kafka integration events when products change.

It is intended to be called through the API gateway, but it also validates JWT tokens itself for defense in depth.

---

## Service URL

- Local: `http://localhost:5001`
- Container internal: `http://productservice:8080`

---

## API Endpoints

All endpoints are under `/api/products`.

| Method | Route | Description |
|---|---|---|
| GET | `/api/products` | List all products |
| GET | `/api/products/{id}` | Get a single product |
| POST | `/api/products` | Create a new product |
| PUT | `/api/products/{id}` | Update an existing product |
| DELETE | `/api/products/{id}` | Delete a product |

> The controller is protected by `[Authorize]`, so requests require a valid Bearer token.

### Product Model (simplified)

```json
{
  "id": "guid",
  "name": "string",
  "price": 99.99,
  "stock": 10,
  "createdAt": "2026-04-06T00:00:00Z"
}
```

---

## Authentication & Authorization

JWT authentication is configured in `Program.cs`.

- Authority: `Jwt:Authority` from configuration
- `ValidateAudience = false`
- `RequireHttpsMetadata = false` (development-friendly)

Authorization policies:

- `UserOnly`: requires role `user`
- `AdminOnly`: requires role `admin`

Keycloak authority is configured in `ProductService/appsettings.json`:

```json
"Jwt": {
  "Authority": "http://localhost:8081/realms/microservices",
  "Audience": "account"
}
```

---

## Database

Persistence uses Entity Framework Core with PostgreSQL.

- DbContext: `ProductService/Data/AppDbContext.cs`
- Connection string: `ConnectionStrings:DefaultConnection`

Default local connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=productsdb;Username=postgres;Password=postgres"
}
```

### Automatic Migrations

The service applies EF Core migrations on startup.
This is convenient for development, but for production you may want to manage migrations separately.

---

## Kafka Integration

The service publishes product events to Kafka.

Configured in `appsettings.json` for containerized deployment:

```json
"Kafka": {
  "BootstrapServers": "kafka1:29092,kafka2:29093,kafka3:29094",
  "Topic": "product-events"
}
```

For local development against the compose cluster, use:

```json
"Kafka": {
  "BootstrapServers": "localhost:9092,localhost:9093,localhost:9094",
  "Topic": "product-events"
}
```

### Event types

- `ProductCreatedEvent`
- `ProductUpdatedEvent`
- `ProductDeletedEvent`

The event types are defined under `ProductService/Events/`.

### Kafka producer

The producer implementation lives in `ProductService/Messaging/`:

- `IKafkaProducer`
- `KafkaProducer`

Messages are serialized as JSON and keyed by `ProductId` when available.

---

## OpenAPI and Scalar UI

The service exposes Swagger/OpenAPI and Scalar documentation.

- Swagger UI: `/swagger`
- OpenAPI document: `/swagger/v1/swagger.json`
- Scalar UI: configured by `app.MapScalarApiReference(...)`

---

## Running Locally

From the repository root:

```bash
cd ProductService
dotnet restore
dotnet run
```

Make sure the required dependencies are available:

- PostgreSQL for `ConnectionStrings:DefaultConnection`
- Keycloak for `Jwt:Authority`
- Kafka for `Kafka:BootstrapServers`

---

## Testing the Service

Example direct calls:

```bash
curl -X GET "http://localhost:5001/api/products" \
  -H "Authorization: Bearer <JWT_TOKEN>"
```

```bash
curl -X POST "http://localhost:5001/api/products" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <JWT_TOKEN>" \
  -d '{"name":"Sample","price":19.99,"stock":50}'
```

---

## Useful Files

- `ProductService/Program.cs`: startup, authentication, EF Core, OpenAPI/Scalar
- `ProductService/Controllers/ProductsController.cs`: CRUD endpoints and event publishing
- `ProductService/Events/`: integration event contracts
- `ProductService/Messaging/`: Kafka producer implementation
- `ProductService/appsettings.json`: persistence, JWT, and Kafka configuration
