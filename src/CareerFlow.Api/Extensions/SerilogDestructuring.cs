using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace CareerFlow.Api.Extensions;

/// <summary>
/// Política de destruturação que mascara dados sensíveis (senhas, tokens, etc.)
/// </summary>
public class SensitiveDataDestructuringPolicy : IDestructuringPolicy
{
    private static readonly HashSet<string> SensitiveProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "passwordhash", "secret", "token", "accesstoken", "refreshtoken",
        "creditcard", "creditcardnumber", "cvv", "ssn", "cpf", "rg"
    };

    public bool TryDestructure(
        object value,
        ILogEventPropertyValueFactory propertyValueFactory,
        out LogEventPropertyValue result)
    {
        if (value is not string strValue)
        {
            result = new ScalarValue(value);
            return false;
        }

        // Se for uma string muito longa, trunca
        if (strValue.Length > 500)
        {
            result = new ScalarValue(strValue[..500] + "... [truncated]");
            return true;
        }

        result = new ScalarValue(value);
        return false;
    }
}

/// <summary>
/// Extensões para registro de políticas de destruturação
/// </summary>
public static class SerilogDestructuringExtensions
{
    public static LoggerConfiguration WithSensitiveDataMasking(
        this LoggerDestructuringConfiguration destructure)
    {
        return destructure.With<SensitiveDataDestructuringPolicy>();
    }
}
