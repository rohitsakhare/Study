# Order Service

The Order Service is a microservice responsible for managing order data in the microservices solution.

## Overview

This service provides RESTful APIs to retrieve order information. It is built using .NET 10.0 and ASP.NET Core.

## Prerequisites

- .NET 10.0 SDK

## Running Locally

1. Navigate to the OrderService directory.
2. Run `dotnet restore`
3. Run `dotnet run`

The service will start on `http://localhost:5002` (or as configured in launchSettings.json).

## API Endpoints

- `GET /api/orders` - Retrieves a list of orders.

Example response:
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

## Docker

To build and run with Docker:

```bash
docker build -t orderservice .
docker run -p 8080:8080 orderservice
```

## Testing

Use the `OrderService.http` file for testing endpoints in VS Code or similar tools.

## Configuration

- `appsettings.json` - Main configuration
- `appsettings.Development.json` - Development-specific settings

## Integration

This service is routed through the API Gateway at `/orders`.