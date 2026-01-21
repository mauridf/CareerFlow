# Dockerfile para Render
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solution e projetos
COPY *.sln .
COPY src/CareerFlow.API/*.csproj src/CareerFlow.API/
COPY src/CareerFlow.Application/*.csproj src/CareerFlow.Application/
COPY src/CareerFlow.Domain/*.csproj src/CareerFlow.Domain/
COPY src/CareerFlow.Infrastructure/*.csproj src/CareerFlow.Infrastructure/

# Restaurar dependências
RUN dotnet restore

# Copiar código fonte
COPY src/. ./src/

# Build da aplicação
WORKDIR /src/CareerFlow.API
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Instalar curl para health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Criar diretório para uploads
RUN mkdir -p /data/uploads && chmod 777 /data/uploads

# Copiar aplicação publicada
COPY --from=build /app/publish .

# Expor porta padrão do Render
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "CareerFlow.API.dll"]