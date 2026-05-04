# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["SaasPlatform.sln", "."]
COPY ["ApiGateway/ApiGateway.csproj", "ApiGateway/"]
COPY ["Services/UserService/UserService.csproj", "Services/UserService/"]
COPY ["Services/OrderService/OrderService.csproj", "Services/OrderService/"]
COPY ["Services/ProductService/ProductService.csproj", "Services/ProductService/"]
COPY ["Shared/DTOs/Shared.DTOs.csproj", "Shared/DTOs/"]
COPY ["Shared/Auth/Shared.Auth.csproj", "Shared/Auth/"]
COPY ["Shared/Messaging/Shared.Messaging.csproj", "Shared/Messaging/"]

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build
RUN dotnet build -c Release -o /app/build

# Publish stage for API Gateway
FROM build as publish-apigateway
RUN dotnet publish "ApiGateway/ApiGateway.csproj" -c Release -o /app/publish

# Publish stage for User Service
FROM build as publish-userservice
RUN dotnet publish "Services/UserService/UserService.csproj" -c Release -o /app/publish

# Publish stage for Order Service
FROM build as publish-orderservice
RUN dotnet publish "Services/OrderService/OrderService.csproj" -c Release -o /app/publish

# Publish stage for Product Service
FROM build as publish-productservice
RUN dotnet publish "Services/ProductService/ProductService.csproj" -c Release -o /app/publish

# Runtime stage for API Gateway
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as apigateway-runtime
WORKDIR /app
COPY --from=publish-apigateway /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ApiGateway.dll"]

# Runtime stage for User Service
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as userservice-runtime
WORKDIR /app
COPY --from=publish-userservice /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "UserService.dll"]

# Runtime stage for Order Service
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as orderservice-runtime
WORKDIR /app
COPY --from=publish-orderservice /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "OrderService.dll"]

# Runtime stage for Product Service
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as productservice-runtime
WORKDIR /app
COPY --from=publish-productservice /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProductService.dll"]
