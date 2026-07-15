namespace CareerFlow.Core.Interfaces.Settings;

/// <summary>
/// Configurações de armazenamento de arquivos
/// </summary>
public class StorageSettings
{
    public const string SectionName = "Storage";

    /// <summary>Provedor de armazenamento (Local, MinIO, S3)</summary>
    public string Provider { get; set; } = "Local";

    /// <summary>Caminho local para uploads</summary>
    public string LocalPath { get; set; } = "uploads";

    /// <summary>Tamanho máximo do arquivo em MB</summary>
    public int MaxFileSizeMB { get; set; } = 5;

    /// <summary>Extensões permitidas</summary>
    public List<string> AllowedExtensions { get; set; } = new() { ".jpg", ".jpeg", ".png", ".webp" };

    /// <summary>Tamanho máximo em bytes</summary>
    public long MaxFileSizeBytes => MaxFileSizeMB * 1024 * 1024;
}
