# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all project files
COPY . .

# Restore dependencies
RUN dotnet restore "ECommerceMicroservices.sln"

# Build
RUN dotnet build "ECommerceMicroservices.sln" -c Release -o /app/build

# Publish
RUN dotnet publish "InventoryService/InventoryService.csproj" -c Release -o /app/publish

# Runtime stage - using slim image for reduced size
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5282
ENTRYPOINT ["dotnet", "InventoryService.dll"]
