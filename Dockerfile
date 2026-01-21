# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln .
COPY src/CareerFlow.API/*.csproj src/CareerFlow.API/
COPY src/CareerFlow.Application/*.csproj src/CareerFlow.Application/
COPY src/CareerFlow.Domain/*.csproj src/CareerFlow.Domain/
COPY src/CareerFlow.Infrastructure/*.csproj src/CareerFlow.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY src/. ./src/

# Build the application
WORKDIR /src/CareerFlow.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install required system dependencies for PostgreSQL
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=build /app/publish .

# Create directory for uploads
RUN mkdir -p uploads && chmod 777 uploads

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "CareerFlow.API.dll"]