namespace CareerFlow.Core.Interfaces;

/// <summary>
/// Interface para barramento de mensagens (RabbitMQ, Outbox, etc.).
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// Publica uma mensagem em uma fila/tópico
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem</typeparam>
    /// <param name="message">Conteúdo da mensagem</param>
    /// <param name="routingKey">Chave de roteamento (opcional)</param>
    Task PublishAsync<T>(T message, string? routingKey = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publica múltiplas mensagens em lote
    /// </summary>
    Task PublishBatchAsync<T>(IEnumerable<T> messages, string? routingKey = null, CancellationToken cancellationToken = default) where T : class;
}
