using MediatR;
using CareerFlow.Application.Features.SocialNetworks.Commands;
using CareerFlow.Application.Features.SocialNetworks.DTOs;
using CareerFlow.Application.Features.SocialNetworks.Queries;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.SocialNetworks.Handlers;

public class CreateSocialNetworkHandler : IRequestHandler<CreateSocialNetworkCommand, SocialNetworkResponse>
{
    private readonly ISocialNetworkRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSocialNetworkHandler(ISocialNetworkRepository repo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _repo = repo; _currentUser = currentUser; _unitOfWork = unitOfWork;
    }

    public async Task<SocialNetworkResponse> Handle(CreateSocialNetworkCommand cmd, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        if (await _repo.CountByPersonIdAsync(personId, ct) >= 10)
            throw new DomainException("Limite máximo de 10 redes sociais atingido");

        var sn = SocialNetwork.Create(personId, cmd.NetworkType, cmd.Url, cmd.DisplayOrder);
        await _repo.AddAsync(sn, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new SocialNetworkResponse
        {
            Id = sn.Id,
            NetworkType = sn.NetworkType.GetDisplayName(),
            Url = sn.Url,
            DisplayOrder = sn.DisplayOrder,
            CreatedAt = sn.CreatedAt
        };
    }
}

public class GetSocialNetworksHandler : IRequestHandler<GetSocialNetworksQuery, IReadOnlyList<SocialNetworkResponse>>
{
    private readonly ISocialNetworkRepository _repo;
    private readonly ICurrentUserService _currentUser;

    public GetSocialNetworksHandler(ISocialNetworkRepository repo, ICurrentUserService currentUser)
    {
        _repo = repo; _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<SocialNetworkResponse>> Handle(GetSocialNetworksQuery req, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var networks = await _repo.GetByPersonIdAsync(personId, ct);
        return networks.Select(s => new SocialNetworkResponse
        {
            Id = s.Id,
            NetworkType = s.NetworkType.GetDisplayName(),
            Url = s.Url,
            DisplayOrder = s.DisplayOrder,
            CreatedAt = s.CreatedAt
        }).ToList().AsReadOnly();
    }
}

public class DeleteSocialNetworkHandler : IRequestHandler<DeleteSocialNetworkCommand>
{
    private readonly ISocialNetworkRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSocialNetworkHandler(ISocialNetworkRepository repo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _repo = repo; _currentUser = currentUser; _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteSocialNetworkCommand cmd, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var sn = await _repo.GetByIdAsync(cmd.Id, ct) ?? throw new NotFoundException("Rede social", cmd.Id);
        if (sn.PersonId != personId) throw new UnauthorizedException();
        _repo.Delete(sn);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}

public class UpdateSocialNetworkHandler : IRequestHandler<UpdateSocialNetworkCommand, SocialNetworkResponse>
{
    private readonly ISocialNetworkRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSocialNetworkHandler(ISocialNetworkRepository repo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _repo = repo; _currentUser = currentUser; _unitOfWork = unitOfWork;
    }

    public async Task<SocialNetworkResponse> Handle(UpdateSocialNetworkCommand cmd, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var sn = await _repo.GetByIdAsync(cmd.Id, ct) ?? throw new NotFoundException("Rede social", cmd.Id);
        if (sn.PersonId != personId) throw new UnauthorizedException();

        sn.Update(cmd.NetworkType, cmd.Url, cmd.DisplayOrder);

        _repo.Update(sn);
        await _unitOfWork.SaveChangesAsync(ct);

        return new SocialNetworkResponse
        {
            Id = sn.Id,
            NetworkType = sn.NetworkType.GetDisplayName(),
            Url = sn.Url,
            DisplayOrder = sn.DisplayOrder,
            CreatedAt = sn.CreatedAt
        };
    }
}
