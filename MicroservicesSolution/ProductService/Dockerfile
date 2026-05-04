# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file only
COPY ProductService.csproj ./
RUN dotnet restore ./ProductService.csproj

# Copy rest of source code
COPY . ./

# Publish
RUN dotnet publish -c Release -o /app/publish \
    /p:UseAppHost=false \
    /p:SelfContained=false 


# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProductService.dll"]