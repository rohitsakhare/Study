# User Service

The User Service is a microservice responsible for managing user data in the microservices solution.

## Overview

This service provides RESTful APIs to retrieve user information. It is built using .NET 10.0 and ASP.NET Core.

## Prerequisites

- .NET 10.0 SDK

## Running Locally

1. Navigate to the UserService directory.
2. Run `dotnet restore`
3. Run `dotnet run`

The service will start on `http://localhost:5000` (or as configured in launchSettings.json).

## API Endpoints

- `GET /api/users` - Retrieves a list of users.

Example response:
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

## Docker

To build and run with Docker:

```bash
docker build -t userservice .
docker run -p 8080:8080 userservice
```

## Testing

Use the `UserService.http` file for testing endpoints in VS Code or similar tools.

## Configuration

- `appsettings.json` - Main configuration
- `appsettings.Development.json` - Development-specific settings

## Integration

This service is routed through the API Gateway at `/users`.