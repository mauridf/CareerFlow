using CareerFlow.Application.DTOs;
using CareerFlow.Domain.Enums;
using FluentValidation;

namespace CareerFlow.Application.Validators;

public class UpdateCertificateDtoValidator : AbstractValidator<UpdateCertificateDto>
{
    public UpdateCertificateDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Data de término deve ser após a data de início")
            .When(x => x.EndDate.HasValue && x.StartDate.HasValue);
    }
}

public class UpdateLanguageDtoValidator : AbstractValidator<UpdateLanguageDto>
{
    public UpdateLanguageDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Level)
            .Must(BeValidLanguageLevel).WithMessage("Nível de idioma inválido")
            .When(x => !string.IsNullOrEmpty(x.Level));
    }

    // CORREÇÃO AQUI: Mudar parâmetro para string?
    private bool BeValidLanguageLevel(string? level)
    {
        // Tratar caso nulo ou vazio
        if (string.IsNullOrEmpty(level))
            return false;

        return LanguageLevel.TryFromName(level, out _);
    }
}

public class UpdateSocialMediaDtoValidator : AbstractValidator<UpdateSocialMediaDto>
{
    public UpdateSocialMediaDtoValidator()
    {
        RuleFor(x => x.Platform)
            .MaximumLength(100).WithMessage("Plataforma deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Platform));

        RuleFor(x => x.Url)
            .MaximumLength(500).WithMessage("URL deve ter no máximo 500 caracteres")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("URL inválida")
            .When(x => !string.IsNullOrEmpty(x.Url));
    }
}