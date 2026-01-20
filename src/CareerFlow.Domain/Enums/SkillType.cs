using Ardalis.SmartEnum;

namespace CareerFlow.Domain.Enums;

public sealed class SkillType : SmartEnum<SkillType>
{
    public static readonly SkillType BACKEND = new SkillType("BACKEND", 1);
    public static readonly SkillType FRONTEND = new SkillType("FRONTEND", 2);
    public static readonly SkillType DATABASE = new SkillType("BANCO DE DADOS", 3);
    public static readonly SkillType CLOUD_DEVOPS = new SkillType("CLOUD E DEVOPS", 4);
    public static readonly SkillType ARCHITECTURE = new SkillType("ARQUITETURA E PADRÕES", 5);
    public static readonly SkillType TOOLS = new SkillType("FERRAMENTAS E OUTRAS TECNOLOGIAS", 6);

    private SkillType(string name, int value) : base(name, value) { }
}