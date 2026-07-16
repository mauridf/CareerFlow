using FluentValidation;
using MediatR;
using CareerFlow.Application.Features.Auth.DTOs;

namespace CareerFlow.Application.Features.Auth.Commands;

public record RegisterUserCommand(
    string Name,
    string Email,
    string Password) : IRequest<AuthResponse>;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(200).WithMessage("Email deve ter no máximo 200 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres")
            .Matches(@"[A-Za-z]").WithMessage("Senha deve conter letras")
            .Matches(@"[0-9]").WithMessage("Senha deve conter números");
    }
}
