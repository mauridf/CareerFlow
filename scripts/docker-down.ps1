#!/usr/bin/env pwsh
# Script para parar ambiente de desenvolvimento

Write-Host "🛑 Parando ambiente CareerFlow..." -ForegroundColor Yellow

# Parar containers
docker compose down

# Opcional: remover volumes (dados)
# docker compose down -v

Write-Host "✅ Ambiente parado." -ForegroundColor Green