using FluentValidation;
using MediatR;

namespace CareerFlow.Application.Features.Auth.Commands;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Senha atual é obrigatória");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Nova senha é obrigatória")
            .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres")
            .Matches(@"[A-Za-z]").WithMessage("Senha deve conter letras")
            .Matches(@"[0-9]").WithMessage("Senha deve conter números")
            .NotEqual(x => x.CurrentPassword).WithMessage("Nova senha deve ser diferente da atual");
    }
}
