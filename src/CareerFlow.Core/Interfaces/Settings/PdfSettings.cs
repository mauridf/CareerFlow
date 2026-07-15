namespace CareerFlow.Core.Interfaces.Settings;

/// <summary>
/// Configurações de geração de PDF
/// </summary>
public class PdfSettings
{
    public const string SectionName = "Pdf";

    /// <summary>Tipo de licença (Community, Professional)</summary>
    public string LicenseType { get; set; } = "Community";

    /// <summary>Tempo máximo de geração em segundos</summary>
    public int MaxGenerationTimeSeconds { get; set; } = 30;

    /// <summary>Tamanho máximo do arquivo PDF em MB</summary>
    public int MaxFileSizeMB { get; set; } = 5;

    /// <summary>Fonte padrão</summary>
    public string DefaultFont { get; set; } = "Arial";
}
