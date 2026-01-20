using Ardalis.SmartEnum;

namespace CareerFlow.Domain.Enums;

public sealed class SkillLevel : SmartEnum<SkillLevel>
{
    public static readonly SkillLevel BASIC = new SkillLevel("BASICO", 1);
    public static readonly SkillLevel INTERMEDIATE = new SkillLevel("INTERMEDIARIO", 2);
    public static readonly SkillLevel ADVANCED = new SkillLevel("AVANÇADO", 3);

    private SkillLevel(string name, int value) : base(name, value) { }
}