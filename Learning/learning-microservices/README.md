# Learning Microservices

A full-stack microservices project demonstrating a product management system with separate backend and frontend services, optimized for production deployment with Docker.

## Architecture

This project consists of the following services:

- **Product Service** (.NET 9): A REST API for managing products, built with ASP.NET Core and Entity Framework Core
- **Product Frontend** (React + TypeScript): A web application that provides a user interface for managing products
- **PostgreSQL Database**: Data persistence layer

All services are containerized with optimized Docker images for efficient deployment and scaling.

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and [Docker Compose](https://docs.docker.com/compose/install/)
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (for local development)
- [Node.js](https://nodejs.org/) (v16 or higher, for local development)
- [PostgreSQL](https://www.postgresql.org/download/) (for local development only)

## Quick Start with Docker (Recommended)

The easiest way to run the entire application is using Docker Compose:

1. Clone the repository and navigate to the project directory:
   ```bash
   cd learning-microservices
   ```

2. Start all services:
   ```bash
   docker-compose up --build
   ```

3. Access the application:
   - **Frontend**: http://localhost:3000
   - **API**: http://localhost:8080
   - **Database**: localhost:5432 (admin/admin)

4. Stop the services:
   ```bash
   docker-compose down
   ```

## Local Development Setup

For development with hot-reloading and debugging capabilities:

### Database Setup

1. Install and start PostgreSQL
2. Create a database named `productsdb`
3. Update the connection string in `product-service/appsettings.json` if needed (default uses localhost:5432 with username/password: postgres/postgres)

### Backend Setup (Product Service)

1. Navigate to the product-service directory:
   ```bash
   cd learning-microservices/product-service
   ```

2. Apply database migrations:
   ```bash
   dotnet ef database update
   ```

3. Run the backend service:
   ```bash
   dotnet run
   ```

The API will be available at `http://localhost:5049` (check `launchSettings.json` for the exact port).

### Frontend Setup (Product Frontend)

1. Open a new terminal and navigate to the product-frontend directory:
   ```bash
   cd learning-microservices/product-frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```

The frontend will be available at `http://localhost:3000`.

## Docker Images

Both services use optimized Docker images for production efficiency:

### Product Service (API)
- **Base Image**: `mcr.microsoft.com/dotnet/aspnet:9.0-alpine`
- **Size**: ~54MB (content size)
- **Optimizations**:
  - Multi-stage build using Alpine Linux
  - Excluded unnecessary files with `.dockerignore`
  - Optimized build process with `--no-restore` flag

### Product Frontend (React App)
- **Base Image**: `nginx:alpine`
- **Size**: ~26MB (content size)
- **Optimizations**:
  - Multi-stage build (Node.js for build, nginx for serving)
  - Alpine-based Node.js image for smaller build stage
  - Enhanced nginx config with gzip compression and caching
  - Excluded development files with `.dockerignore`

## API Endpoints

The Product Service provides the following REST endpoints:

- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create a new product
- `PUT /api/products/{id}` - Update a product
- `DELETE /api/products/{id}` - Delete a product

### Testing the API

Use the `product-service.http` file in the product-service directory to test the endpoints directly. This file contains sample requests for all operations.

## Project Structure

```
learning-microservices/
├── docker-compose.yml          # Docker Compose configuration
├── product-service/            # .NET 9 Web API
│   ├── Controllers/
│   │   └── ProductsController.cs
│   ├── Data/
│   │   └── ProductDbContext.cs
│   ├── Models/
│   │   └── Product.cs
│   ├── Migrations/
│   ├── Dockerfile              # Optimized Alpine-based image
│   ├── .dockerignore           # Excludes unnecessary files
│   ├── appsettings.json
│   ├── Program.cs
│   ├── product-service.http
│   └── product-service.csproj
├── product-frontend/           # React TypeScript App
│   ├── src/
│   │   ├── components/
│   │   │   ├── ProductForm.tsx
│   │   │   └── ProductList.tsx
│   │   ├── api.ts
│   │   ├── types.ts
│   │   ├── App.tsx
│   │   └── index.tsx
│   ├── Dockerfile              # Multi-stage optimized build
│   ├── .dockerignore           # Excludes development files
│   ├── nginx.conf              # Optimized nginx config with gzip & caching
│   ├── package.json
│   └── tsconfig.json
└── README.md
```

## Technologies Used

### Backend
- .NET 9 with ASP.NET Core Web API
- Entity Framework Core with PostgreSQL
- Npgsql (PostgreSQL provider)
- Docker with Alpine Linux for optimized images

### Frontend
- React 19 with TypeScript
- Axios for API communication
- Create React App (development)
- Nginx with Alpine Linux for production serving
- Gzip compression and static asset caching

### Infrastructure
- Docker & Docker Compose for containerization
- PostgreSQL database
- Multi-stage Docker builds for optimization

## Development Workflow

### Docker Development (Recommended)
1. Start all services: `docker-compose up --build`
2. Access the application at `http://localhost:3000`
3. Make changes to code - containers will rebuild automatically
4. Stop services: `docker-compose down`

### Local Development
1. Start PostgreSQL database
2. Run backend service (`dotnet run` in product-service)
3. Run frontend (`npm start` in product-frontend)
4. Access the application at `http://localhost:3000`

The frontend communicates with the backend API to perform CRUD operations on products.

## Troubleshooting

### Docker Issues
If `docker-compose up` fails:
- Ensure Docker and Docker Compose are installed and running
- Check that ports 3000, 8080, and 5432 are not already in use
- Clear Docker cache: `docker system prune -a`
- Rebuild without cache: `docker-compose build --no-cache`

### Backend Issues
If `dotnet run` fails:
- Ensure PostgreSQL is running and the database exists
- Check the connection string in `appsettings.json`
- Run `dotnet ef database update` to apply migrations
- Verify .NET 9 SDK is installed: `dotnet --version`

### Frontend Issues
If `npm start` fails:
- Ensure Node.js is installed: `node --version`
- Run `npm install` to install dependencies
- Check that the backend is running on the expected port (5049)

### Connection Issues
- Verify the API base URL in `product-frontend/src/api.ts` matches your backend port
- Check browser console for CORS or network errors
- For Docker setup, ensure services are communicating via container names (e.g., `http://product-api:8080`)

## Performance Optimizations

This project includes several Docker image optimizations for production efficiency:

- **Alpine Linux base images**: Reduced attack surface and smaller size
- **Multi-stage builds**: Separate build and runtime environments
- **Strategic .dockerignore files**: Exclude unnecessary files from build context
- **Nginx optimizations**: Gzip compression and static asset caching
- **Efficient layer caching**: Optimized Dockerfile instructions for better build performance

These optimizations result in significantly smaller and faster container images suitable for production deployment.
