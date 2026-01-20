using Ardalis.SmartEnum;

namespace CareerFlow.Domain.Enums;

public sealed class EducationLevel : SmartEnum<EducationLevel>
{
    public static readonly EducationLevel GRADUATION = new EducationLevel("GRADUAÇÃO", 1);
    public static readonly EducationLevel POSTGRADUATION = new EducationLevel("PÓS-GRADUAÇÃO", 2);
    public static readonly EducationLevel MBA = new EducationLevel("MBA", 3);
    public static readonly EducationLevel MASTERS = new EducationLevel("MESTRADO", 4);
    public static readonly EducationLevel DOCTORATE = new EducationLevel("DOUTORADO", 5);
    public static readonly EducationLevel TECHNICIAN = new EducationLevel("TÉCNICO", 6);

    private EducationLevel(string name, int value) : base(name, value) { }
}