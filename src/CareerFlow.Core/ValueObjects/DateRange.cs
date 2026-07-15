namespace CareerFlow.Core.ValueObjects;

/// <summary>
/// Value Object que representa um período de datas.
/// Usado em experiências e formação acadêmica.
/// </summary>
public sealed class DateRange : ValueObject
{
    /// <summary>Data de início</summary>
    public DateTime StartDate { get; }

    /// <summary>Data de término (null = atual/em andamento)</summary>
    public DateTime? EndDate { get; }

    /// <summary>Período em andamento (sem data de término)</summary>
    public bool IsOngoing => !EndDate.HasValue;

    /// <summary>Duração em meses</summary>
    public int DurationInMonths
    {
        get
        {
            var end = EndDate ?? DateTime.UtcNow;
            return ((end.Year - StartDate.Year) * 12) + end.Month - StartDate.Month;
        }
    }

    /// <summary>Duração formatada (ex: "2 anos e 3 meses")</summary>
    public string DurationFormatted
    {
        get
        {
            var totalMonths = DurationInMonths;
            var years = totalMonths / 12;
            var months = totalMonths % 12;

            if (years == 0)
                return $"{months} {(months == 1 ? "mês" : "meses")}";

            if (months == 0)
                return $"{years} {(years == 1 ? "ano" : "anos")}";

            return $"{years} {(years == 1 ? "ano" : "anos")} e {months} {(months == 1 ? "mês" : "meses")}";
        }
    }

    /// <summary>
    /// Cria um novo período com validação
    /// </summary>
    public DateRange(DateTime startDate, DateTime? endDate = null)
    {
        if (startDate > DateTime.UtcNow)
            throw new ArgumentException("Data de início não pode ser no futuro", nameof(startDate));

        if (endDate.HasValue)
        {
            if (endDate.Value > DateTime.UtcNow)
                throw new ArgumentException("Data de término não pode ser no futuro", nameof(endDate));

            if (startDate > endDate.Value)
                throw new ArgumentException("Data de início deve ser anterior à data de término");
        }

        StartDate = startDate.Date;
        EndDate = endDate?.Date;
    }

    /// <summary>
    /// Verifica se uma data está dentro do período
    /// </summary>
    public bool Contains(DateTime date)
    {
        return date >= StartDate && (!EndDate.HasValue || date <= EndDate.Value);
    }

    /// <summary>
    /// Verifica se o período se sobrepõe a outro
    /// </summary>
    public bool Overlaps(DateRange other)
    {
        return StartDate <= (other.EndDate ?? DateTime.MaxValue) &&
               (EndDate ?? DateTime.MaxValue) >= other.StartDate;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    public override string ToString()
    {
        var start = StartDate.ToString("MMM/yyyy");
        var end = EndDate?.ToString("MMM/yyyy") ?? "Atual";
        return $"{start} - {end}";
    }
}
