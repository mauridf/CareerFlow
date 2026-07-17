using FluentValidation;
using MediatR;

namespace CareerFlow.Application.Features.Auth.Commands;

public record VerifyEmailCommand(string Token) : IRequest;

public class VerifyEmailValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage("Token é obrigatório");
    }
}
