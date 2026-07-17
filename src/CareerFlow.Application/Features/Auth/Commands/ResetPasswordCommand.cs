using FluentValidation;
using MediatR;

namespace CareerFlow.Application.Features.Auth.Commands;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage("Token é obrigatório");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Nova senha é obrigatória")
            .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres")
            .Matches(@"[A-Za-z]").WithMessage("Senha deve conter letras")
            .Matches(@"[0-9]").WithMessage("Senha deve conter números");
    }
}
