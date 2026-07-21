using FluentValidation;

namespace CareerFlow.Application.Features.Certificates.Commands;

public class UpdateCertificateCommandValidator : AbstractValidator<UpdateCertificateCommand>
{
    public UpdateCertificateCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Título é obrigatório")
            .MaximumLength(300).WithMessage("Título deve ter no máximo 300 caracteres");

        RuleFor(x => x.Issuer)
            .NotEmpty().WithMessage("Emissor é obrigatório")
            .MaximumLength(200).WithMessage("Emissor deve ter no máximo 200 caracteres");

        When(x => x.ExpirationDate.HasValue, () =>
        {
            RuleFor(x => x.IssueDate).LessThan(x => x.ExpirationDate!.Value)
                .WithMessage("Data de emissão deve ser anterior à expiração");
        });
    }
}
