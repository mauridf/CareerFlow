using MediatR;
using CareerFlow.Application.Features.SocialNetworks.DTOs;

namespace CareerFlow.Application.Features.SocialNetworks.Queries;

public record GetSocialNetworksQuery : IRequest<IReadOnlyList<SocialNetworkResponse>>;
