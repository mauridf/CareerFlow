using FluentValidation;
using MediatR;
using CareerFlow.Application.Features.Certificates.DTOs;

namespace CareerFlow.Application.Features.Certificates.Commands;

public record CreateCertificateCommand(
    string Title, string Issuer, DateTime IssueDate, DateTime? ExpirationDate,
    string? CertificateId, string? CredentialId, string? CredentialUrl) : IRequest<CertificateResponse>;

public class CreateCertificateValidator : AbstractValidator<CreateCertificateCommand>
{
    public CreateCertificateValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Título é obrigatório").MaximumLength(300);
        RuleFor(x => x.Issuer).NotEmpty().WithMessage("Emissor é obrigatório").MaximumLength(200);
        When(x => x.ExpirationDate.HasValue, () =>
            RuleFor(x => x.IssueDate).LessThan(x => x.ExpirationDate!.Value)
                .WithMessage("Data de emissão deve ser anterior à expiração"));
    }
}

public record UpdateCertificateCommand(
    Guid Id, string Title, string Issuer, DateTime IssueDate,
    DateTime? ExpirationDate, string? CertificateId, string? CredentialUrl) : IRequest<CertificateResponse>;

public record DeleteCertificateCommand(Guid Id) : IRequest;
