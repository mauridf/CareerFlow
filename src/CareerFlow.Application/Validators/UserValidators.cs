using CareerFlow.Application.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace CareerFlow.Application.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
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
            .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres")
            .MaximumLength(100).WithMessage("Senha deve ter no máximo 100 caracteres")
            .Matches(@"[A-Z]+").WithMessage("Senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[a-z]+").WithMessage("Senha deve conter pelo menos uma letra minúscula")
            .Matches(@"[0-9]+").WithMessage("Senha deve conter pelo menos um número")
            .Matches(@"[\!\?\*\.\@\#\$\%\^\&\+\=]+").WithMessage("Senha deve conter pelo menos um caractere especial (!?*.@#$%^&+=)");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.State)
            .Length(2).WithMessage("UF deve ter 2 caracteres")
            .When(x => !string.IsNullOrEmpty(x.State));
    }
}

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.State)
            .Length(2).WithMessage("UF deve ter 2 caracteres")
            .When(x => !string.IsNullOrEmpty(x.State));

        RuleFor(x => x.Photo)
            .Must(BeAValidImage).WithMessage("Foto deve ser uma imagem válida (JPG, PNG)")
            .When(x => x.Photo != null);
    }

    private bool BeAValidImage(IFormFile? file)
    {
        if (file == null) return true;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(extension);
    }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória");
    }
}