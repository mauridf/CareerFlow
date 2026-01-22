# Dockerfile CORRIGIDO para .NET 10 e Render
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar arquivos de solução (.sln) e projetos
COPY *.sln ./
COPY src/CareerFlow.API/*.csproj src/CareerFlow.API/
COPY src/CareerFlow.Application/*.csproj src/CareerFlow.Application/
COPY src/CareerFlow.Domain/*.csproj src/CareerFlow.Domain/
COPY src/CareerFlow.Infrastructure/*.csproj src/CareerFlow.Infrastructure/

# Verificar se há arquivos copiados (para debug)
RUN ls -la && ls -la src/CareerFlow.API/

# Restaurar dependências especificando o arquivo de solução
RUN dotnet restore "CareerFlow.sln"

# Copiar código fonte
COPY src/. ./src/

# Build da aplicação
WORKDIR /src/CareerFlow.API
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Instalar curl para health checks (versão mais leve)
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*

# Criar diretório para uploads (temporário no free tier)
RUN mkdir -p /tmp/uploads && chmod 777 /tmp/uploads

# Copiar aplicação publicada
COPY --from=build /app/publish .

# Expor porta padrão do Render
EXPOSE 8080

# Health check simplificado para Render
HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "CareerFlow.API.dll"]