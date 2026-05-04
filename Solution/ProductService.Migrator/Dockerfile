FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./ProductService/ProductService.csproj && dotnet restore ./ProductService.Migrator/ProductService.Migrator.csproj
RUN dotnet publish ./ProductService.Migrator/ProductService.Migrator.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ProductService.Migrator.dll"]