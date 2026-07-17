namespace CareerFlow.Core.Interfaces.Settings;

public class EmailSettings
{
    public const string SectionName = "Email";

    public string SmtpHost { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromName { get; set; } = "CareerFlow";
    public string FromEmail { get; set; } = "noreply@careerflow.com";
    public bool UseSsl { get; set; } = false;
    public string FrontendUrl { get; set; } = "http://localhost:3000";
}
