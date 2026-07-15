#!/usr/bin/env pwsh
# Script para iniciar ambiente de desenvolvimento

Write-Host "🚀 Iniciando ambiente CareerFlow..." -ForegroundColor Cyan

# Verificar Docker
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Docker não encontrado. Instale o Docker Desktop." -ForegroundColor Red
    exit 1
}

# Subir containers
docker compose up -d

# Aguardar health checks
Write-Host "⏳ Aguardando serviços ficarem saudáveis..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# Verificar status
docker compose ps

Write-Host "✅ Ambiente pronto!" -ForegroundColor Green
Write-Host "   PostgreSQL: localhost:5432" -ForegroundColor White
Write-Host "   Redis:      localhost:6379" -ForegroundColor White