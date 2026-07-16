namespace CareerFlow.Core.Enums;

/// <summary>
/// Tipos de vínculo empregatício.
/// </summary>
public enum EmploymentType
{
    /// <summary>Tempo integral (CLT)</summary>
    FullTime = 0,

    /// <summary>Meio período</summary>
    PartTime = 1,

    /// <summary>Contrato temporário</summary>
    Contract = 2,

    /// <summary>Estágio</summary>
    Internship = 3,

    /// <summary>Freelance/PJ</summary>
    Freelance = 4,

    /// <summary>Trabalho remoto</summary>
    Remote = 5,

    /// <summary>Voluntário</summary>
    Volunteer = 6
}

public static class EmploymentTypeExtensions
{
    private static readonly Dictionary<EmploymentType, string> DisplayNames = new()
    {
        { EmploymentType.FullTime, "Tempo Integral" },
        { EmploymentType.PartTime, "Meio Período" },
        { EmploymentType.Contract, "Contrato" },
        { EmploymentType.Internship, "Estágio" },
        { EmploymentType.Freelance, "Freelance/PJ" },
        { EmploymentType.Remote, "Remoto" },
        { EmploymentType.Volunteer, "Voluntário" }
    };

    public static string GetDisplayName(this EmploymentType type)
    {
        return DisplayNames.TryGetValue(type, out var name) ? name : type.ToString();
    }
}
