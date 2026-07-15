using System.Text.RegularExpressions;

namespace CareerFlow.Core.ValueObjects;

/// <summary>
/// Value Object que representa um número de telefone brasileiro.
/// Formato aceito: (XX) XXXXX-XXXX ou (XX) XXXX-XXXX
/// </summary>
public sealed class PhoneNumber : ValueObject
{
    /// <summary>Número completo formatado</summary>
    public string Value { get; }

    /// <summary>DDD (2 dígitos)</summary>
    public string DDD => Value.Length >= 2 ? Value[..2] : string.Empty;

    /// <summary>Número sem DDD</summary>
    public string Number => Value.Length > 2 ? Value[2..] : string.Empty;

    /// <summary>Número apenas com dígitos</summary>
    public string DigitsOnly { get; }

    private static readonly Regex PhoneRegex = new(
        @"^\(?([1-9]{2})\)?\s?9?[0-9]{4}-?[0-9]{4}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Cria um novo PhoneNumber com validação
    /// </summary>
    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Telefone não pode ser vazio", nameof(value));

        // Remove tudo que não for dígito
        DigitsOnly = new string(value.Where(char.IsDigit).ToArray());

        if (DigitsOnly.Length < 10 || DigitsOnly.Length > 11)
            throw new ArgumentException("Telefone deve ter 10 ou 11 dígitos (com DDD)", nameof(value));

        // Valida DDD (não pode começar com 0)
        var ddd = DigitsOnly[..2];
        if (ddd.StartsWith("0") || !int.TryParse(ddd, out var dddNum) || dddNum < 11 || dddNum > 99)
            throw new ArgumentException("DDD inválido", nameof(value));

        // Formata o número
        if (DigitsOnly.Length == 11)
        {
            // Celular: (XX) 9XXXX-XXXX
            Value = $"({DigitsOnly[..2]}) {DigitsOnly[2..7]}-{DigitsOnly[7..]}";
        }
        else
        {
            // Fixo: (XX) XXXX-XXXX
            Value = $"({DigitsOnly[..2]}) {DigitsOnly[2..6]}-{DigitsOnly[6..]}";
        }
    }

    private PhoneNumber(string value, bool skipValidation)
    {
        Value = value;
        DigitsOnly = new string(value.Where(char.IsDigit).ToArray());
    }

    public static PhoneNumber FromDatabase(string value)
    {
        return new PhoneNumber(value, skipValidation: true);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return DigitsOnly;
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phone) => phone.Value;
    public static explicit operator PhoneNumber(string value) => new(value);
}
