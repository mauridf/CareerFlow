using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Features.Profile.DTOs;
using CareerFlow.Application.Features.Profile.Queries;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Profile.Handlers;

public class GetProfileHandler : IRequestHandler<GetProfileQuery, ProfileResponse>
{
    private readonly IPersonRepository _personRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetProfileHandler> _logger;

    public GetProfileHandler(
        IPersonRepository personRepository,
        ICurrentUserService currentUser,
        ILogger<GetProfileHandler> logger)
    {
        _personRepository = personRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();

        _logger.LogDebug("🔍 Buscando perfil: {UserId}", userId);

        var person = await _personRepository.GetFullProfileAsync(
            (await _personRepository.GetByUserIdAsync(userId, cancellationToken))?.Id ?? Guid.Empty,
            cancellationToken)
            ?? throw new NotFoundException("Perfil");

        return new ProfileResponse
        {
            Id = person.Id,
            UserId = person.UserId,
            Phone = person.Phone,
            City = person.City,
            State = person.State,
            BirthDate = person.BirthDate,
            ProfessionalSummary = person.ProfessionalSummary,
            PhotoUrl = person.PhotoUrl,
            CurrentPosition = person.CurrentPosition,
            CurrentCompany = person.CurrentCompany,
            IsPublic = person.IsPublic,
            ResumeSlug = person.ResumeSlug,
            CompletionPercentage = person.CalculateCompletionPercentage(),
            CanGenerateResume = person.CanGenerateResume(),
            CreatedAt = person.CreatedAt,
            UpdatedAt = person.UpdatedAt
        };
    }
}
