FROM mcr.microsoft.com/dotnet/sdk:9.0.100 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./ProductReadService/ProductReadService.csproj
RUN dotnet publish ./ProductReadService/ProductReadService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProductReadService.dll"]