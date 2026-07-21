using FluentValidation;
using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.Education.Commands;

public class UpdateEducationCommandValidator : AbstractValidator<UpdateEducationCommand>
{
    public UpdateEducationCommandValidator()
    {
        RuleFor(x => x.Institution)
            .NotEmpty().WithMessage("Instituição é obrigatória");

        RuleFor(x => x.Course)
            .NotEmpty().WithMessage("Curso é obrigatório");

        RuleFor(x => x.EducationLevel)
            .IsInEnum().WithMessage("Nível de formação inválido");
    }
}
