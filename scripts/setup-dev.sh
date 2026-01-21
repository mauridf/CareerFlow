#!/bin/bash

echo "🚀 Configurando ambiente de desenvolvimento do CareerFlow..."

# Verificar se o Docker está instalado
if ! command -v docker &> /dev/null; then
    echo "❌ Docker não está instalado. Por favor, instale o Docker primeiro."
    exit 1
fi

# Verificar se o dotnet está instalado
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK não está instalado. Por favor, instale o .NET 8.0 SDK."
    exit 1
fi

# Criar diretórios necessários
mkdir -p uploads
mkdir -p logs

# Restaurar pacotes
echo "📦 Restaurando pacotes NuGet..."
dotnet restore

# Aplicar migrations
echo "🗄️ Aplicando migrations do banco de dados..."
cd src/CareerFlow.Infrastructure
dotnet ef database update --startup-project ../CareerFlow.API

# Voltar para a raiz
cd ../..

echo "✅ Configuração concluída!"
echo ""
echo "Para iniciar a aplicação:"
echo "1. Com Docker Compose: docker-compose up"
echo "2. Localmente: cd src/CareerFlow.API && dotnet run"
echo ""
echo "📚 Endpoints importantes:"
echo "   - API: http://localhost:8080"
echo "   - Swagger: http://localhost:8080/swagger"
echo "   - Health Check: http://localhost:8080/health"