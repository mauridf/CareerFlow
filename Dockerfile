# Dockerfile (atualizado)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY src/CareerFlow.API/*.csproj src/CareerFlow.API/
COPY src/CareerFlow.Application/*.csproj src/CareerFlow.Application/
COPY src/CareerFlow.Domain/*.csproj src/CareerFlow.Domain/
COPY src/CareerFlow.Infrastructure/*.csproj src/CareerFlow.Infrastructure/

RUN dotnet restore

COPY src/. ./src/

WORKDIR /src/CareerFlow.API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Instalar curl
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Criar diretório TEMPORÁRIO para uploads (dentro do container)
RUN mkdir -p /tmp/uploads && chmod 777 /tmp/uploads

COPY --from=build /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "CareerFlow.API.dll"]