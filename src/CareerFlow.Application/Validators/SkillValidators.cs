using CareerFlow.Application.DTOs;
using CareerFlow.Domain.Enums;
using FluentValidation;

namespace CareerFlow.Application.Validators;

public class CreateSkillDtoValidator : AbstractValidator<CreateSkillDto>
{
    public CreateSkillDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome da habilidade é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Tipo da habilidade é obrigatório")
            .Must(BeValidSkillType).WithMessage("Tipo de habilidade inválido");

        RuleFor(x => x.Level)
            .NotEmpty().WithMessage("Nível da habilidade é obrigatório")
            .Must(BeValidSkillLevel).WithMessage("Nível de habilidade inválido");
    }

    private bool BeValidSkillType(string type)
    {
        return SkillType.TryFromName(type, out _);
    }

    private bool BeValidSkillLevel(string level)
    {
        return SkillLevel.TryFromName(level, out _);
    }
}