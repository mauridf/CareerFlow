# Dockerfile FINAL - FUNCIONA
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copiar APENAS os arquivos .csproj primeiro
COPY src/CareerFlow.API/CareerFlow.API.csproj src/CareerFlow.API/
COPY src/CareerFlow.Application/CareerFlow.Application.csproj src/CareerFlow.Application/
COPY src/CareerFlow.Domain/CareerFlow.Domain.csproj src/CareerFlow.Domain/
COPY src/CareerFlow.Infrastructure/CareerFlow.Infrastructure.csproj src/CareerFlow.Infrastructure/

# 2. Criar .sln APENAS SE necessário
RUN if [ ! -f "*.sln" ]; then \
    echo "Criando CareerFlow.sln..." && \
    dotnet new sln --name CareerFlow; \
    fi

# 3. Adicionar projetos ao .sln (se não estiverem lá)
RUN if [ -f "CareerFlow.sln" ]; then \
    echo "Adicionando projetos ao CareerFlow.sln..." && \
    dotnet sln CareerFlow.sln add src/CareerFlow.API/CareerFlow.API.csproj 2>/dev/null || true && \
    dotnet sln CareerFlow.sln add src/CareerFlow.Application/CareerFlow.Application.csproj 2>/dev/null || true && \
    dotnet sln CareerFlow.sln add src/CareerFlow.Domain/CareerFlow.Domain.csproj 2>/dev/null || true && \
    dotnet sln CareerFlow.sln add src/CareerFlow.Infrastructure/CareerFlow.Infrastructure.csproj 2>/dev/null || true; \
    fi

# 4. Restaurar especificando o .sln
RUN if [ -f "CareerFlow.sln" ]; then \
    dotnet restore CareerFlow.sln; \
    else \
    echo "Nenhum .sln encontrado, restaurando projetos individualmente" && \
    dotnet restore src/CareerFlow.API/CareerFlow.API.csproj && \
    dotnet restore src/CareerFlow.Application/CareerFlow.Application.csproj && \
    dotnet restore src/CareerFlow.Domain/CareerFlow.Domain.csproj && \
    dotnet restore src/CareerFlow.Infrastructure/CareerFlow.Infrastructure.csproj; \
    fi

# 5. Copiar TODO o código fonte
COPY src/. ./src/

# 6. Build
WORKDIR /src/src/CareerFlow.API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
RUN mkdir -p /tmp/uploads && chmod 777 /tmp/uploads
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "CareerFlow.API.dll"]