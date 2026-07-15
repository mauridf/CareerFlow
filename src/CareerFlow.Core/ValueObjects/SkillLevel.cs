namespace CareerFlow.Core.ValueObjects;

/// <summary>
/// Value Object que representa o nível de uma habilidade.
/// Combina categoria e nível de proficiência com validação.
/// </summary>
public sealed class SkillLevel : ValueObject
{
    /// <summary>Categoria da habilidade</summary>
    public string Category { get; }

    /// <summary>Nível de proficiência</summary>
    public string Level { get; }

    /// <summary>Score numérico (0-100)</summary>
    public int Score { get; }

    /// <summary>Descrição do nível</summary>
    public string Description => GetDescription();

    private static readonly Dictionary<string, int> LevelScores = new()
    {
        { "basic", 25 },
        { "intermediate", 50 },
        { "advanced", 75 },
        { "expert", 100 }
    };

    private static readonly Dictionary<string, string> LevelDescriptions = new()
    {
        { "basic", "Conhecimento básico, necessita supervisão" },
        { "intermediate", "Conhecimento intermediário, trabalha de forma independente" },
        { "advanced", "Conhecimento avançado, capaz de mentorar outros" },
        { "expert", "Especialista, referência na área" }
    };

    /// <summary>
    /// Cria um novo SkillLevel
    /// </summary>
    /// <param name="category">Categoria da habilidade</param>
    /// <param name="level">Nível (basic, intermediate, advanced, expert)</param>
    public SkillLevel(string category, string level)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Categoria é obrigatória", nameof(category));

        if (string.IsNullOrWhiteSpace(level))
            throw new ArgumentException("Nível é obrigatório", nameof(level));

        level = level.ToLowerInvariant().Trim();

        if (!LevelScores.ContainsKey(level))
            throw new ArgumentException(
                $"Nível inválido. Níveis aceitos: {string.Join(", ", LevelScores.Keys)}",
                nameof(level));

        Category = category.Trim();
        Level = level;
        Score = LevelScores[level];
    }

    private string GetDescription()
    {
        return LevelDescriptions.TryGetValue(Level, out var desc)
            ? desc
            : "Nível não especificado";
    }

    /// <summary>
    /// Retorna o nível como percentual
    /// </summary>
    public int GetPercentage() => Score;

    /// <summary>
    /// Níveis disponíveis para exibição em UI
    /// </summary>
    public static IReadOnlyDictionary<string, int> AvailableLevels => LevelScores;

    /// <summary>
    /// Nomes amigáveis dos níveis em português
    /// </summary>
    public static readonly Dictionary<string, string> LevelNamesPtBr = new()
    {
        { "basic", "Básico" },
        { "intermediate", "Intermediário" },
        { "advanced", "Avançado" },
        { "expert", "Especialista" }
    };

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Category.ToLowerInvariant();
        yield return Level;
    }

    public override string ToString() => $"{Category} - {Level} ({Score}%)";
}
