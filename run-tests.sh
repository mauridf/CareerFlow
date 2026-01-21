#!/bin/bash

echo "🧪 Executando testes..."

# Executar testes unitários
echo "📋 Testes de Domain..."
dotnet test tests/CareerFlow.Domain.Tests --verbosity quiet

echo "📋 Testes de Application..."
dotnet test tests/CareerFlow.Application.Tests --verbosity quiet

echo "📋 Testes de API..."
dotnet test tests/CareerFlow.API.Tests --verbosity quiet

echo "✅ Todos os testes foram executados!"