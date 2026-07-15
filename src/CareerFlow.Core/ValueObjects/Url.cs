namespace CareerFlow.Core.ValueObjects;

/// <summary>
/// Value Object que representa uma URL válida.
/// </summary>
public sealed class Url : ValueObject
{
    /// <summary>URL completa</summary>
    public string Value { get; }

    /// <summary>Domínio da URL</summary>
    public string Host => new Uri(Value).Host;

    /// <summary>Esquema (http/https)</summary>
    public string Scheme => new Uri(Value).Scheme;

    /// <summary>
    /// Cria uma nova URL com validação
    /// </summary>
    public Url(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("URL não pode ser vazia", nameof(value));

        value = value.Trim();

        if (value.Length > 500)
            throw new ArgumentException("URL deve ter no máximo 500 caracteres", nameof(value));

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            throw new ArgumentException("URL inválida", nameof(value));

        if (uri.Scheme != "http" && uri.Scheme != "https")
            throw new ArgumentException("URL deve usar esquema HTTP ou HTTPS", nameof(value));

        Value = value;
    }

    private Url(string value, bool skipValidation)
    {
        Value = value;
    }

    public static Url FromDatabase(string value)
    {
        return new Url(value, skipValidation: true);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value.ToLowerInvariant();
    }

    public override string ToString() => Value;

    public static implicit operator string(Url url) => url.Value;
    public static explicit operator Url(string value) => new(value);
}
