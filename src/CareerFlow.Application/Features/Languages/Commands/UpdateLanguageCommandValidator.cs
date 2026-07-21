using FluentValidation;
using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.Languages.Commands;

public class UpdateLanguageCommandValidator : AbstractValidator<UpdateLanguageCommand>
{
    public UpdateLanguageCommandValidator()
    {
        RuleFor(x => x.LanguageName)
            .NotEmpty().WithMessage("Nome do idioma é obrigatório")
            .MaximumLength(50).WithMessage("Nome do idioma deve ter no máximo 50 caracteres");

        RuleFor(x => x.ProficiencyLevel)
            .IsInEnum().WithMessage("Nível de proficiência inválido");
    }
}
