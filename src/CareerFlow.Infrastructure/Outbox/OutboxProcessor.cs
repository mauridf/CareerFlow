using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Outbox;

/// <summary>
/// Background service que processa mensagens do Outbox.
/// Lê mensagens pendentes e publica no barramento.
/// </summary>
public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(10);
    private readonly int _batchSize = 20;

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📤 OutboxProcessor iniciado. Intervalo: {Interval}s, Batch: {BatchSize}",
            _processingInterval.TotalSeconds, _batchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao processar mensagens do Outbox");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("📤 OutboxProcessor finalizado");
    }

    /// <summary>
    /// Processa mensagens pendentes no Outbox
    /// </summary>
    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CareerFlowDbContext>();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        // Busca mensagens pendentes (ordenadas por data de criação)
        var messages = await dbContext.OutboxMessages
            .Where(m => m.Status == "pending" && m.RetryCount < m.MaxRetries)
            .OrderBy(m => m.CreatedAt)
            .Take(_batchSize)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0) return;

        _logger.LogDebug("📨 {Count} mensagens pendentes encontradas no Outbox", messages.Count);

        foreach (var message in messages)
        {
            try
            {
                message.MarkAsProcessing();
                await dbContext.SaveChangesAsync(cancellationToken);

                // Publica a mensagem no barramento
                await messageBus.PublishAsync(
                    message.Content,
                    routingKey: message.Type,
                    cancellationToken: cancellationToken);

                message.MarkAsProcessed();
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogDebug("✅ Mensagem {Id} ({Type}) processada com sucesso", message.Id, message.Type);
            }
            catch (Exception ex)
            {
                message.MarkAsFailed(ex.Message, ex.StackTrace);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogWarning(ex, "⚠️ Falha ao processar mensagem {Id} ({Type}). Tentativa {Retry}/{MaxRetries}",
                    message.Id, message.Type, message.RetryCount, message.MaxRetries);
            }
        }
    }
}
