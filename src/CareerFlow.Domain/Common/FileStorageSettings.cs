namespace CareerFlow.Domain.Common;

public class FileStorageSettings
{
    public string BasePath { get; set; } = "uploads";
    public int MaxFileSizeMB { get; set; } = 5;
    public List<string> AllowedExtensions { get; set; } = new();
}