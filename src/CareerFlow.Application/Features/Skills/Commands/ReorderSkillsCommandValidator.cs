using FluentValidation;

namespace CareerFlow.Application.Features.Skills.Commands;

public class ReorderSkillsCommandValidator : AbstractValidator<ReorderSkillsCommand>
{
    public ReorderSkillsCommandValidator()
    {
        RuleFor(x => x.Skills)
            .NotEmpty().WithMessage("Lista de habilidades é obrigatória");

        RuleForEach(x => x.Skills)
            .ChildRules(skill =>
            {
                skill.RuleFor(x => x.Id)
                    .NotEmpty().WithMessage("ID da habilidade é obrigatório");

                skill.RuleFor(x => x.DisplayOrder)
                    .GreaterThanOrEqualTo(0).WithMessage("Ordem de exibição deve ser maior ou igual a zero");
            });
    }
}
