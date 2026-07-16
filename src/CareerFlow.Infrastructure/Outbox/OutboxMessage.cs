namespace CareerFlow.Infrastructure.Outbox;

/// <summary>
/// Entidade que representa uma mensagem no Outbox Pattern.
/// Garante consistência eventual entre banco de dados e mensageria.
/// </summary>
public class OutboxMessage
{
    /// <summary>Identificador único da mensagem</summary>
    public Guid Id { get; set; }

    /// <summary>Tipo da mensagem (nome do evento)</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Conteúdo da mensagem em JSON</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Headers adicionais (JSON)</summary>
    public string Headers { get; set; } = "{}";

    /// <summary>Status atual (pending, processing, processed, failed)</summary>
    public string Status { get; set; } = "pending";

    /// <summary>Contador de tentativas de envio</summary>
    public int RetryCount { get; set; }

    /// <summary>Número máximo de tentativas</summary>
    public int MaxRetries { get; set; } = 5;

    /// <summary>Último erro ocorrido</summary>
    public string? LastError { get; set; }

    /// <summary>Stack trace do último erro</summary>
    public string? ErrorStackTrace { get; set; }

    /// <summary>Data de criação</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Data de processamento</summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>Data de envio</summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Cria uma nova mensagem de outbox
    /// </summary>
    public static OutboxMessage Create<T>(T message, string type, string? headers = null)
    {
        var content = System.Text.Json.JsonSerializer.Serialize(message);

        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = type,
            Content = content,
            Headers = headers ?? "{}",
            Status = "pending",
            RetryCount = 0,
            MaxRetries = 5,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Marca a mensagem como processada com sucesso
    /// </summary>
    public void MarkAsProcessed()
    {
        Status = "processed";
        ProcessedAt = DateTime.UtcNow;
        SentAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca a mensagem como falha
    /// </summary>
    public void MarkAsFailed(string error, string? stackTrace = null)
    {
        RetryCount++;

        if (RetryCount >= MaxRetries)
        {
            Status = "failed";
        }
        else
        {
            Status = "pending"; // Volta para tentar novamente
        }

        LastError = error;
        ErrorStackTrace = stackTrace;
    }

    /// <summary>
    /// Marca a mensagem como em processamento
    /// </summary>
    public void MarkAsProcessing()
    {
        Status = "processing";
    }

    /// <summary>
    /// Verifica se a mensagem pode ser processada
    /// </summary>
    public bool CanProcess()
    {
        return Status == "pending" && RetryCount < MaxRetries;
    }
}
