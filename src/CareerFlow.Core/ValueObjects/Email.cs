using System.Text.RegularExpressions;

namespace CareerFlow.Core.ValueObjects;

/// <summary>
/// Value Object que representa um endereço de email.
/// Encapsula a validação de formato de email.
/// </summary>
public sealed class Email : ValueObject
{
    /// <summary>Valor do email</summary>
    public string Value { get; }

    /// <summary>Parte local (antes do @)</summary>
    public string LocalPart => Value.Split('@')[0];

    /// <summary>Domínio (depois do @)</summary>
    public string Domain => Value.Split('@')[1];

    /// <summary>Email normalizado (minúsculo, sem espaços)</summary>
    public string NormalizedValue => Value.ToLowerInvariant().Trim();

    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Cria um novo Email com validação
    /// </summary>
    /// <param name="value">Endereço de email</param>
    /// <exception cref="ArgumentException">Se o email for inválido</exception>
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email não pode ser vazio", nameof(value));

        value = value.Trim();

        if (value.Length > 200)
            throw new ArgumentException("Email deve ter no máximo 200 caracteres", nameof(value));

        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Formato de email inválido", nameof(value));

        // Verifica domínios comuns com erros de digitação
        var domain = value.Split('@')[1].ToLowerInvariant();
        var commonTypos = new Dictionary<string, string>
        {
            { "gmail.con", "gmail.com" },
            { "gmail.co", "gmail.com" },
            { "gmial.com", "gmail.com" },
            { "hotmail.con", "hotmail.com" },
            { "hotmail.co", "hotmail.com" },
            { "outlook.con", "outlook.com" },
            { "outlook.co", "outlook.com" },
            { "yaho.com", "yahoo.com" },
            { "yahoo.con", "yahoo.com" }
        };

        if (commonTypos.TryGetValue(domain, out var correctedDomain))
        {
            var localPart = value.Split('@')[0];
            Value = $"{localPart}@{correctedDomain}";
        }
        else
        {
            Value = value;
        }
    }

    /// <summary>
    /// Cria um Email sem validação (uso interno, ex: do banco de dados)
    /// </summary>
    private Email(string value, bool skipValidation)
    {
        Value = value;
    }

    /// <summary>
    /// Cria um Email a partir de um valor já validado (para ORM)
    /// </summary>
    public static Email FromDatabase(string value)
    {
        return new Email(value, skipValidation: true);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return NormalizedValue;
    }

    public override string ToString() => Value;

    /// <summary>
    /// Conversão implícita de string para Email (cuidado: pode lançar exceção)
    /// </summary>
    public static implicit operator string(Email email) => email.Value;

    /// <summary>
    /// Conversão explícita de Email para string
    /// </summary>
    public static explicit operator Email(string value) => new(value);
}
