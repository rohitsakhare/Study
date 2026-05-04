# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore first for caching
COPY ["ApiGateway.csproj", "./"]
RUN dotnet restore "ApiGateway.csproj"

# Copy everything else
COPY . .

# Publish the app (include ocelot.json)
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:CopyOutputSymbolsToPublishDirectory=false /p:CopyOutputRefsToPublishDirectory=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Run the app
ENTRYPOINT ["dotnet", "ApiGateway.dll"]