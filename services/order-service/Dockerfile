# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY OrderService.sln ./
COPY src/OrderService.Api.csproj src/
COPY tests/unit/OrderService.Api.Tests.csproj tests/unit/
RUN dotnet restore OrderService.sln
COPY . .
RUN dotnet publish src/OrderService.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Api.dll"]
