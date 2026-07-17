using FluentValidation;
using MediatR;
using CareerFlow.Application.Features.SocialNetworks.DTOs;
using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.SocialNetworks.Commands;

public record CreateSocialNetworkCommand(SocialNetworkType NetworkType, string Url, int DisplayOrder) : IRequest<SocialNetworkResponse>;
public record UpdateSocialNetworkCommand(Guid Id, SocialNetworkType NetworkType, string Url, int DisplayOrder) : IRequest<SocialNetworkResponse>;
public record DeleteSocialNetworkCommand(Guid Id) : IRequest;

public class CreateSocialNetworkValidator : AbstractValidator<CreateSocialNetworkCommand>
{
    public CreateSocialNetworkValidator()
    {
        RuleFor(x => x.Url).NotEmpty().WithMessage("URL é obrigatória")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("URL inválida");
        RuleFor(x => x.NetworkType).IsInEnum();
    }
}
