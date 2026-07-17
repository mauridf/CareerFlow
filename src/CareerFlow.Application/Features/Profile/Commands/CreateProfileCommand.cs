using FluentValidation;
using MediatR;
using CareerFlow.Application.Features.Profile.DTOs;

namespace CareerFlow.Application.Features.Profile.Commands;

public record CreateProfileCommand(
    string? Phone,
    string? City,
    string? State,
    DateTime? BirthDate,
    string? ProfessionalSummary,
    string? CurrentPosition,
    string? CurrentCompany) : IRequest<ProfileResponse>;

public class CreateProfileValidator : AbstractValidator<CreateProfileCommand>
{
    public CreateProfileValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.ProfessionalSummary), () =>
        {
            RuleFor(x => x.ProfessionalSummary)
                .MinimumLength(100).WithMessage("Resumo profissional deve ter no mínimo 100 caracteres")
                .MaximumLength(2000).WithMessage("Resumo profissional deve ter no máximo 2000 caracteres");
        });

        When(x => !string.IsNullOrWhiteSpace(x.State), () =>
        {
            RuleFor(x => x.State)
                .Length(2).WithMessage("Estado deve ter 2 caracteres (UF)");
        });

        When(x => x.BirthDate.HasValue, () =>
        {
            RuleFor(x => x.BirthDate!.Value)
                .LessThan(DateTime.Now.AddYears(-14)).WithMessage("Usuário deve ter pelo menos 14 anos");
        });
    }
}
