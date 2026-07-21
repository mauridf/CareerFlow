using FluentValidation;

namespace CareerFlow.Application.Features.Experiences.Commands;

public class UpdateExperienceCommandValidator : AbstractValidator<UpdateExperienceCommand>
{
    public UpdateExperienceCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Nome da empresa é obrigatório");

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Cargo é obrigatório");

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
