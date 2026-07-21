using FluentValidation;
using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.SocialNetworks.Commands;

public class UpdateSocialNetworkCommandValidator : AbstractValidator<UpdateSocialNetworkCommand>
{
    public UpdateSocialNetworkCommandValidator()
    {
        RuleFor(x => x.NetworkType)
            .IsInEnum().WithMessage("Tipo de rede social inválido");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("URL é obrigatória")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("URL inválida");
    }
}
