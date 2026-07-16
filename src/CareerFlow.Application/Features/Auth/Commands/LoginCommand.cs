using FluentValidation;
using MediatR;
using CareerFlow.Application.Features.Auth.DTOs;

namespace CareerFlow.Application.Features.Auth.Commands;

public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponse>;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória");
    }
}
