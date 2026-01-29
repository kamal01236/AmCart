# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY SearchService.sln ./
COPY src/SearchService.Api.csproj src/
COPY tests/SearchService.Api.Tests.csproj tests/
RUN dotnet restore SearchService.sln
COPY . .
RUN dotnet publish src/SearchService.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SearchService.Api.dll"]
