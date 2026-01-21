#!/bin/bash
# scripts/deploy-render.sh

echo "🚀 PREPARANDO DEPLOY PARA RENDER.COM"
echo "====================================="

# 1. Verificar se está no diretório correto
if [ ! -f "CareerFlow.sln" ]; then
    echo "❌ ERRO: Execute este script da raiz do projeto!"
    exit 1
fi

# 2. Verificar arquivos necessários
echo "📁 Verificando arquivos necessários..."
required_files=("Dockerfile" "render.yaml" "src/CareerFlow.API/appsettings.Production.json")
for file in "${required_files[@]}"; do
    if [ ! -f "$file" ]; then
        echo "❌ Faltando: $file"
        exit 1
    fi
done
echo "✅ Todos os arquivos necessários encontrados"

# 3. Atualizar git
echo "📦 Atualizando repositório Git..."
git add .
read -p "Digite a mensagem do commit: " commit_message
git commit -m "$commit_message"
git push origin main

echo ""
echo "✅ PREPARAÇÃO LOCAL CONCLUÍDA!"
echo ""
echo "📋 PASSO A PASSO PARA DEPLOY NO RENDER:"
echo ""
echo "1. 🌐 Acesse: https://dashboard.render.com"
echo "2. 📦 Clique em 'New +' no topo direito"
echo "3. 🔗 Selecione 'Blueprint'"
echo "4. 🔗 Conecte sua conta GitHub"
echo "5. 📁 Selecione seu repositório: CareerFlow-API"
echo "6. 🚀 Clique em 'Apply'"
echo "7. ⏳ Aguarde o deploy (5-10 minutos)"
echo ""
echo "🔧 CONFIGURAÇÕES AUTOMÁTICAS:"
echo "   • PostgreSQL será criado automaticamente"
echo "   • Volume para uploads será configurado"
echo "   • Health check em /health"
echo ""
echo "⚙️ VARIÁVEIS DE AMBIENTE NECESSÁRIAS (configurar no Render após deploy):"
echo "   JWT_SECRET=gerar_uma_chave_segura_de_32_caracteres"
echo "   JWT_ISSUER=CareerFlowAPI"
echo "   JWT_AUDIENCE=CareerFlowUsers"
echo "   JWT_EXPIRY_MINUTES=60"
echo ""
echo "🔗 Sua API estará disponível em: https://careerflow-api.onrender.com"
echo "📚 Swagger UI: https://careerflow-api.onrender.com/swagger"
echo "🏥 Health Check: https://careerflow-api.onrender.com/health"