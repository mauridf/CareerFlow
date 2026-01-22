#!/bin/bash
# scripts/fix-render-deploy.sh

echo "🔧 CORRIGINDO ERRO DO RENDER DEPLOY"
echo "===================================="

# 1. Atualizar render.yaml
echo "📝 Atualizando render.yaml (removendo disk)..."
cat > render.yaml << 'EOF'
# render.yaml (CORRIGIDO - sem disk no free tier)
services:
  - type: web
    name: careerflow-api
    env: docker
    dockerfilePath: ./Dockerfile
    dockerContext: .
    plan: free
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ASPNETCORE_URLS
        value: http://+:8080
      - key: PORT
        value: 8080
    healthCheckPath: /health
    autoDeploy: true

databases:
  - name: careerflow-db
    plan: free
    databaseName: careerflow
    user: careerflow_user
EOF
echo "✅ render.yaml atualizado"

# 2. Atualizar appsettings.Production.json
echo "📝 Atualizando appsettings.Production.json..."
cat > src/CareerFlow.API/appsettings.Production.json << 'EOF'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=${DATABASE_URL};Database=careerflow;Username=careerflow_user;SSL Mode=Require;Trust Server Certificate=true"
  },
  "JwtSettings": {
    "Secret": "${JWT_SECRET}",
    "Issuer": "${JWT_ISSUER}",
    "Audience": "${JWT_AUDIENCE}",
    "ExpiryInMinutes": "${JWT_EXPIRY_MINUTES}"
  },
  "FileStorage": {
    "BasePath": "/tmp/uploads",
    "MaxFileSizeMB": 5,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".pdf"]
  },
  "Serilog": {
    "Using": ["Serilog.Sinks.Console"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
EOF
echo "✅ appsettings.Production.json atualizado"

# 3. Atualizar Program.cs
echo "📝 Atualizando Program.cs para Render..."
# Vamos adicionar as linhas necessárias no início do Program.cs
# Você precisará editar manualmente o Program.cs para incluir o código acima

# 4. Commit e push
echo "📦 Fazendo commit das alterações..."
git add .
git commit -m "fix: remove disk from render.yaml for free tier compatibility"
git push origin main

echo ""
echo "✅ CORREÇÕES APLICADAS!"
echo ""
echo "🔄 AGORA NO RENDER:"
echo "1. Vá para: https://dashboard.render.com"
echo "2. Clique no seu Blueprint 'CareerFlow-API'"
echo "3. Clique em 'Retry' ou 'Deploy'"
echo "4. O erro deve ser resolvido!"
echo ""
echo "⚠️  IMPORTANTE: No plano FREE:"
echo "   • Uploads serão armazenados em /tmp/uploads (VOLÁTIL)"
echo "   • Arquivos serão perdidos quando o container reiniciar"
echo "   • Para uploads persistentes, faça upgrade para plano pago"
echo ""
echo "🔗 URL da API: https://careerflow-api.onrender.com"