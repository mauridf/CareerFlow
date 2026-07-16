using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Specifications;

/// <summary>
/// Especificação que verifica se um perfil está completo.
/// Perfil completo = todos os campos obrigatórios preenchidos.
/// </summary>
public class CompleteProfileSpecification : BaseSpecification<Person>
{
    /// <summary>
    /// Percentual mínimo de completude (0-100)
    /// </summary>
    public int MinimumPercentage { get; }

    /// <summary>
    /// Cria uma especificação com percentual mínimo padrão (80%)
    /// </summary>
    public CompleteProfileSpecification() : this(80)
    {
    }

    /// <summary>
    /// Cria uma especificação com percentual mínimo específico
    /// </summary>
    /// <param name="minimumPercentage">Percentual mínimo (0-100)</param>
    public CompleteProfileSpecification(int minimumPercentage)
        : base(person => true) // O critério é avaliado no método IsSatisfiedBy
    {
        MinimumPercentage = minimumPercentage;
    }

    /// <summary>
    /// Verifica se o perfil atende ao percentual mínimo
    /// </summary>
    public new bool IsSatisfiedBy(Person person)
    {
        if (person == null) return false;

        var percentage = person.CalculateCompletionPercentage();
        return percentage >= MinimumPercentage;
    }

    /// <summary>
    /// Retorna os campos que faltam preencher
    /// </summary>
    public List<string> GetMissingFields(Person person)
    {
        var missingFields = new List<string>();

        if (person == null)
        {
            missingFields.Add("Perfil não criado");
            return missingFields;
        }

        if (string.IsNullOrWhiteSpace(person.Phone))
            missingFields.Add("Telefone");

        if (string.IsNullOrWhiteSpace(person.City))
            missingFields.Add("Cidade");

        if (string.IsNullOrWhiteSpace(person.State))
            missingFields.Add("Estado");

        if (!person.BirthDate.HasValue)
            missingFields.Add("Data de nascimento");

        if (string.IsNullOrWhiteSpace(person.ProfessionalSummary))
            missingFields.Add("Resumo profissional");

        if (string.IsNullOrWhiteSpace(person.PhotoUrl))
            missingFields.Add("Foto de perfil");

        if (string.IsNullOrWhiteSpace(person.CurrentPosition))
            missingFields.Add("Cargo atual");

        if (string.IsNullOrWhiteSpace(person.CurrentCompany))
            missingFields.Add("Empresa atual");

        return missingFields;
    }
}
