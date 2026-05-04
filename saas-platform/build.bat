@echo off

echo Building SaaS Platform Solutions...

REM Build all projects
dotnet build SaasPlatform.sln -c Release

if %errorlevel% neq 0 (
    echo Build failed!
    exit /b 1
)

echo Build completed successfully!
echo.
echo Building Docker images...

REM Build API Gateway
docker build -t saas-platform/apigateway:latest --target apigateway-runtime .

REM Build User Service
docker build -t saas-platform/userservice:latest --target userservice-runtime .

REM Build Order Service
docker build -t saas-platform/orderservice:latest --target orderservice-runtime .

REM Build Product Service
docker build -t saas-platform/productservice:latest --target productservice-runtime .

echo.
echo All Docker images built successfully!
echo.
echo To start all services, run:
echo docker-compose up -d
