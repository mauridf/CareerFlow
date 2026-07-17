using FluentValidation;
using MediatR;
using CareerFlow.Application.Features.Skills.DTOs;
using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.Skills.Commands;

public record CreateSkillCommand(
    string Name,
    SkillCategory Category,
    ProficiencyLevel ProficiencyLevel = ProficiencyLevel.Basic,
    bool IsPrimary = false,
    int DisplayOrder = 0) : IRequest<SkillResponse>;

public class CreateSkillValidator : AbstractValidator<CreateSkillCommand>
{
    public CreateSkillValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome da habilidade é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Categoria inválida");

        RuleFor(x => x.ProficiencyLevel)
            .IsInEnum().WithMessage("Nível de proficiência inválido");
    }
}

public record UpdateSkillCommand(
    Guid Id,
    string Name,
    SkillCategory Category,
    ProficiencyLevel ProficiencyLevel,
    bool IsPrimary,
    int DisplayOrder) : IRequest<SkillResponse>;

public record DeleteSkillCommand(Guid Id) : IRequest;

public record ReorderSkillsCommand(List<SkillOrderItem> Skills) : IRequest;
