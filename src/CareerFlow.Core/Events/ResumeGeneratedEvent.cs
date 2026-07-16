namespace CareerFlow.Core.Events;

/// <summary>
/// Evento disparado quando um currículo PDF é gerado.
/// </summary>
public sealed class ResumeGeneratedEvent : DomainEvent
{
    public Guid PersonId { get; }
    public Guid UserId { get; }
    public string FileName { get; }
    public long FileSizeBytes { get; }
    public bool IsAtsOptimized { get; }

    public ResumeGeneratedEvent(Guid personId, Guid userId, string fileName, long fileSizeBytes, bool isAtsOptimized)
    {
        PersonId = personId;
        UserId = userId;
        FileName = fileName;
        FileSizeBytes = fileSizeBytes;
        IsAtsOptimized = isAtsOptimized;
    }
}
