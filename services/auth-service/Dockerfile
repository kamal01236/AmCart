# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY AuthService.sln ./
COPY src/AuthService.Api.csproj src/
COPY tests/AuthService.Api.Tests.csproj tests/
RUN dotnet restore AuthService.sln
COPY . .
RUN dotnet publish src/AuthService.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthService.Api.dll"]
