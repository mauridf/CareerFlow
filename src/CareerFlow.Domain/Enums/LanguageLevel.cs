using Ardalis.SmartEnum;

namespace CareerFlow.Domain.Enums;

public sealed class LanguageLevel : SmartEnum<LanguageLevel>
{
    public static readonly LanguageLevel BASIC = new LanguageLevel("BASICO", 1);
    public static readonly LanguageLevel INTERMEDIATE = new LanguageLevel("INTERMEDIÁRIO", 2);
    public static readonly LanguageLevel ADVANCED = new LanguageLevel("AVANÇADO", 3);
    public static readonly LanguageLevel FLUENT = new LanguageLevel("FLUENTE/NATIVO", 4);

    private LanguageLevel(string name, int value) : base(name, value) { }
}