using FluentValidation;
using MediatR;
using CareerFlow.Application.Features.Experiences.DTOs;
using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.Experiences.Commands;

public record CreateExperienceCommand(
    string CompanyName,
    string Position,
    DateTime StartDate,
    DateTime? EndDate,
    string? Description,
    List<Guid>? SkillsUsed,
    string? City,
    string? State,
    string? Country,
    EmploymentType? EmploymentType) : IRequest<ExperienceResponse>;

public class CreateExperienceValidator : AbstractValidator<CreateExperienceCommand>
{
    public CreateExperienceValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().WithMessage("Nome da empresa é obrigatório");
        RuleFor(x => x.Position).NotEmpty().WithMessage("Cargo é obrigatório");

        When(x => x.EndDate.HasValue, () =>
        {
            RuleFor(x => x.StartDate).LessThan(x => x.EndDate!.Value)
                .WithMessage("Data de início deve ser anterior à data de término");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Description), () =>
        {
            RuleFor(x => x.Description).MinimumLength(50)
                .WithMessage("Descrição deve ter no mínimo 50 caracteres");
        });
    }
}

public record UpdateExperienceCommand(
    Guid Id,
    string CompanyName,
    string Position,
    DateTime StartDate,
    DateTime? EndDate,
    string? Description,
    List<Guid>? SkillsUsed,
    string? City,
    string? State,
    EmploymentType? EmploymentType) : IRequest<ExperienceResponse>;

public record DeleteExperienceCommand(Guid Id) : IRequest;
