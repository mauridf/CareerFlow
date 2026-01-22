# Dockerfile CORRETO
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copiar arquivos de solução (.sln) se existir
COPY *.sln ./

# 2. Copiar projetos - ATENÇÃO aos caminhos!
COPY src/CareerFlow.API/CareerFlow.API.csproj src/CareerFlow.API/
COPY src/CareerFlow.Application/CareerFlow.Application.csproj src/CareerFlow.Application/
COPY src/CareerFlow.Domain/CareerFlow.Domain.csproj src/CareerFlow.Domain/
COPY src/CareerFlow.Infrastructure/CareerFlow.Infrastructure.csproj src/CareerFlow.Infrastructure/

# 3. Se não tem .sln, criar um
RUN if [ ! -f "*.sln" ]; then \
    dotnet new sln --name CareerFlow && \
    dotnet sln CareerFlow.sln add src/CareerFlow.API/CareerFlow.API.csproj && \
    dotnet sln CareerFlow.sln add src/CareerFlow.Application/CareerFlow.Application.csproj && \
    dotnet sln CareerFlow.sln add src/CareerFlow.Domain/CareerFlow.Domain.csproj && \
    dotnet sln CareerFlow.sln add src/CareerFlow.Infrastructure/CareerFlow.Infrastructure.csproj; \
    fi

# 4. Restaurar dependências
RUN dotnet restore

# 5. Copiar código fonte - CORRETO: de ./src para ./src
COPY src/ ./src/

# 6. Build - IMPORTANTE: O projeto está em src/CareerFlow.API/
WORKDIR /src/src/CareerFlow.API  # <-- DUPLO src!
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Criar diretório para uploads
RUN mkdir -p /tmp/uploads && chmod 777 /tmp/uploads

# Copiar aplicação publicada
COPY --from=build /app/publish .

# Expor porta
EXPOSE 8080

# Health check (sem curl - mais simples)
HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "CareerFlow.API.dll"]