#!/bin/bash
# scripts/fix-render-build.sh

echo "🔧 CORRIGINDO ERRO DE BUILD NO RENDER"
echo "======================================"

echo "📝 1. Verificando arquivos de solução..."
if [ -f "CareerFlow.slnx" ]; then
    echo "✅ Encontrado CareerFlow.slnx"
    # Criar um .sln temporário se necessário
    if [ ! -f "CareerFlow.sln" ]; then
        echo "⚠️  Criando CareerFlow.sln temporário..."
        dotnet new sln --name CareerFlow --force
        dotnet sln add src/CareerFlow.API/CareerFlow.API.csproj
        dotnet sln add src/CareerFlow.Application/CareerFlow.Application.csproj
        dotnet sln add src/CareerFlow.Domain/CareerFlow.Domain.csproj
        dotnet sln add src/CareerFlow.Infrastructure/CareerFlow.Infrastructure.csproj
        echo "✅ CareerFlow.sln criado"
    fi
fi

echo "📝 2. Atualizando Dockerfile..."
cat > Dockerfile << 'EOF'
# Dockerfile para .NET 10 e Render
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar solution
COPY *.sln ./
COPY src/CareerFlow.API/*.csproj src/CareerFlow.API/
COPY src/CareerFlow.Application/*.csproj src/CareerFlow.Application/
COPY src/CareerFlow.Domain/*.csproj src/CareerFlow.Domain/
COPY src/CareerFlow.Infrastructure/*.csproj src/CareerFlow.Infrastructure/

# Restaurar dependências
RUN dotnet restore "CareerFlow.sln"

# Copiar código fonte
COPY src/. ./src/

# Build da aplicação
WORKDIR /src/CareerFlow.API
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Instalar curl
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

# Criar diretório para uploads
RUN mkdir -p /tmp/uploads && chmod 777 /tmp/uploads

# Copiar aplicação
COPY --from=build /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "CareerFlow.API.dll"]
EOF
echo "✅ Dockerfile atualizado"

echo "📝 3. Criando .dockerignore..."
cat > .dockerignore << 'EOF'
**/.git
**/.vs
**/.vscode
**/bin
**/obj
**/node_modules
uploads/
logs/
*.log
*.user
*.suo
*.cache
.DS_Store
Thumbs.db
EOF
echo "✅ .dockerignore criado"

echo "📝 4. Atualizando arquivos .csproj para .NET 10..."

# Atualizar CareerFlow.API.csproj
cat > src/CareerFlow.API/CareerFlow.API.csproj << 'EOF'
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="DotNetEnv" Version="3.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CareerFlow.Application\CareerFlow.Application.csproj" />
    <ProjectReference Include="..\CareerFlow.Domain\CareerFlow.Domain.csproj" />
    <ProjectReference Include="..\CareerFlow.Infrastructure\CareerFlow.Infrastructure.csproj" />
  </ItemGroup>
</Project>
EOF

# Atualizar CareerFlow.Application.csproj
cat > src/CareerFlow.Application/CareerFlow.Application.csproj << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CareerFlow.Domain\CareerFlow.Domain.csproj" />
  </ItemGroup>
</Project>
EOF

# Atualizar CareerFlow.Domain.csproj
cat > src/CareerFlow.Domain/CareerFlow.Domain.csproj << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Ardalis.SmartEnum" Version="8.0.0" />
  </ItemGroup>
</Project>
EOF

# Atualizar CareerFlow.Infrastructure.csproj
cat > src/CareerFlow.Infrastructure/CareerFlow.Infrastructure.csproj << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CareerFlow.Application\CareerFlow.Application.csproj" />
    <ProjectReference Include="..\CareerFlow.Domain\CareerFlow.Domain.csproj" />
  </ItemGroup>
</Project>
EOF

echo "✅ Arquivos .csproj atualizados para .NET 10"

echo ""
echo "📦 5. Fazendo commit das alterações..."
git add .
git commit -m "fix: update to .NET 10 and fix Dockerfile for Render deploy"
git push origin main

echo ""
echo "✅ CORREÇÕES APLICADAS!"
echo ""
echo "🔄 AGORA NO RENDER:"
echo "1. Vá para: https://dashboard.render.com"
echo "2. Clique no serviço 'careerflow-api'"
echo "3. Clique em 'Manual Deploy' → 'Deploy latest commit'"
echo "4. Aguarde o build (agora deve funcionar!)"
echo ""
echo "⚙️  VERIFIQUE AS VARIÁVEIS DE AMBIENTE:"
echo "   • DATABASE_URL (deve estar configurado pelo Blueprint)"
echo "   • JWT_SECRET (defina uma chave segura)"
echo "   • JWT_ISSUER, JWT_AUDIENCE, JWT_EXPIRY_MINUTES"
echo ""
echo "🔗 URL da API: https://careerflow-api.onrender.com"