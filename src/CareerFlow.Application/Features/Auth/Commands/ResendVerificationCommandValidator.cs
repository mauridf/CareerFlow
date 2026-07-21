using FluentValidation;

namespace CareerFlow.Application.Features.Auth.Commands;

public class ResendVerificationCommandValidator : AbstractValidator<ResendVerificationCommand>
{
    public ResendVerificationCommandValidator()
    {
    }
}
