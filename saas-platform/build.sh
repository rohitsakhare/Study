#!/bin/bash

echo "Building SaaS Platform Solutions..."

# Build all projects
dotnet build SaasPlatform.sln -c Release

if [ $? -eq 0 ]; then
    echo "Build completed successfully!"
else
    echo "Build failed!"
    exit 1
fi

echo ""
echo "Building Docker images..."

# Build API Gateway
docker build -t saas-platform/apigateway:latest --target apigateway-runtime .

# Build User Service
docker build -t saas-platform/userservice:latest --target userservice-runtime .

# Build Order Service
docker build -t saas-platform/orderservice:latest --target orderservice-runtime .

# Build Product Service
docker build -t saas-platform/productservice:latest --target productservice-runtime .

echo ""
echo "All Docker images built successfully!"
echo ""
echo "To start all services, run:"
echo "docker-compose up -d"
