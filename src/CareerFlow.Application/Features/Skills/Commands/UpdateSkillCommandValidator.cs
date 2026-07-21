using FluentValidation;
using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.Skills.Commands;

public class UpdateSkillCommandValidator : AbstractValidator<UpdateSkillCommand>
{
    public UpdateSkillCommandValidator()
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
