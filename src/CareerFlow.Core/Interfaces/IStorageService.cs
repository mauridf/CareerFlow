namespace CareerFlow.Core.Interfaces;

/// <summary>
/// Serviço para armazenamento de arquivos (local, MinIO, S3).
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Faz upload de um arquivo
    /// </summary>
    /// <param name="fileName">Nome do arquivo</param>
    /// <param name="content">Conteúdo do arquivo</param>
    /// <param name="contentType">Tipo MIME</param>
    /// <param name="folder">Pasta opcional</param>
    /// <returns>URL pública do arquivo</returns>
    Task<string> UploadAsync(string fileName, Stream content, string contentType, string? folder = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um arquivo
    /// </summary>
    /// <param name="filePath">Caminho do arquivo</param>
    Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um arquivo existe
    /// </summary>
    Task<bool> ExistsAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém a URL pública de um arquivo
    /// </summary>
    Task<string> GetPublicUrlAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gera um nome único para o arquivo
    /// </summary>
    string GenerateUniqueFileName(string originalFileName);
}
