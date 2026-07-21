using FluentValidation;

namespace CareerFlow.Application.Features.Auth.Commands;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token é obrigatório");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token é obrigatório");
    }
}
