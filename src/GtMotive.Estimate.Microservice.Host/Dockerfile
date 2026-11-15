# Build context: the root of the repository (see docker-compose.yml).

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /repo

# Build configuration shared by every project (package versions, analyzers, code style).
COPY global.json NuGet.config Directory.Build.props Directory.Build.targets .editorconfig ./

# Project files first, so the restore layer is reused while only the code changes.
COPY ["src/GtMotive.Estimate.Microservice.Host/GtMotive.Estimate.Microservice.Host.csproj", "src/GtMotive.Estimate.Microservice.Host/"]
COPY ["src/GtMotive.Estimate.Microservice.Api/GtMotive.Estimate.Microservice.Api.csproj", "src/GtMotive.Estimate.Microservice.Api/"]
COPY ["src/GtMotive.Estimate.Microservice.ApplicationCore/GtMotive.Estimate.Microservice.ApplicationCore.csproj", "src/GtMotive.Estimate.Microservice.ApplicationCore/"]
COPY ["src/GtMotive.Estimate.Microservice.Domain/GtMotive.Estimate.Microservice.Domain.csproj", "src/GtMotive.Estimate.Microservice.Domain/"]
COPY ["src/GtMotive.Estimate.Microservice.Infrastructure/GtMotive.Estimate.Microservice.Infrastructure.csproj", "src/GtMotive.Estimate.Microservice.Infrastructure/"]
RUN dotnet restore "src/GtMotive.Estimate.Microservice.Host/GtMotive.Estimate.Microservice.Host.csproj"

COPY src/ src/

FROM build AS publish
RUN dotnet publish "src/GtMotive.Estimate.Microservice.Host/GtMotive.Estimate.Microservice.Host.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GtMotive.Estimate.Microservice.Host.dll"]
