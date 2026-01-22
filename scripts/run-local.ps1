# run-local.ps1 (Windows)
Write-Host "🚀 Iniciando CareerFlow API Localmente..." -ForegroundColor Green
Write-Host "📁 Ambiente: Development" -ForegroundColor Yellow
Write-Host "🔗 Banco: PostgreSQL local" -ForegroundColor Cyan

# Verifica se o PostgreSQL está rodando
try {
    $pgTest = & "psql" "-U" "postgres" "-c" "SELECT 1;" 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ PostgreSQL está rodando" -ForegroundColor Green
    } else {
        Write-Host "❌ PostgreSQL não está disponível" -ForegroundColor Red
        Write-Host "💡 Execute: pg_ctl start" -ForegroundColor Yellow
        exit 1
    }
} catch {
    Write-Host "⚠️  PostgreSQL não encontrado no PATH" -ForegroundColor Yellow
}

# Limpa logs antigos
if (Test-Path "logs") {
    Get-ChildItem "logs" -Filter "*.txt" | Remove-Item -Force
}

# Inicia a aplicação
dotnet run --launch-profile "https"