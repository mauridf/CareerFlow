using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Outbox;

/// <summary>
/// Processador de mensagens do Outbox.
/// Executa como um serviço em background, processando mensagens pendentes.
/// </summary>
public class OutboxProcessor : IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(10);
    private readonly int _batchSize = 20;
    private Timer? _timer;
    private bool _disposed;

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Inicia o processamento periódico
    /// </summary>
    public void Start()
    {
        _logger.LogInformation("📤 OutboxProcessor iniciado. Intervalo: {Interval}s, Batch: {BatchSize}",
            _processingInterval.TotalSeconds, _batchSize);

        _timer = new Timer(
            async _ => await ProcessOutboxMessagesAsync(),
            null,
            TimeSpan.Zero,
            _processingInterval);
    }

    /// <summary>
    /// Para o processamento
    /// </summary>
    public void Stop()
    {
        _timer?.Change(Timeout.Infinite, 0);
        _logger.LogInformation("📤 OutboxProcessor parado");
    }

    private async Task ProcessOutboxMessagesAsync()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CareerFlowDbContext>();

            // Busca mensagens pendentes
            var messages = await dbContext.OutboxMessages
                .Where(m => m.Status == "pending" && m.RetryCount < m.MaxRetries)
                .OrderBy(m => m.CreatedAt)
                .Take(_batchSize)
                .ToListAsync();

            if (messages.Count == 0) return;

            _logger.LogDebug("📨 {Count} mensagens pendentes encontradas no Outbox", messages.Count);

            foreach (var message in messages)
            {
                try
                {
                    message.MarkAsProcessing();
                    await dbContext.SaveChangesAsync();

                    // Simula envio (IMessageBus será implementado futuramente)
                    await Task.Delay(100); // Simula latência de rede

                    message.MarkAsProcessed();
                    await dbContext.SaveChangesAsync();

                    _logger.LogDebug("✅ Mensagem {Id} ({Type}) processada", message.Id, message.Type);
                }
                catch (Exception ex)
                {
                    message.MarkAsFailed(ex.Message, ex.StackTrace);
                    await dbContext.SaveChangesAsync();

                    _logger.LogWarning(ex,
                        "⚠️ Falha ao processar mensagem {Id} ({Type}). Tentativa {Retry}/{MaxRetries}",
                        message.Id, message.Type, message.RetryCount, message.MaxRetries);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao processar mensagens do Outbox");
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _timer?.Dispose();
            _disposed = true;
        }
    }
}
