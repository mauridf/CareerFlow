using CareerFlow.Application.Features.Profile.Commands;
using CareerFlow.Application.Features.Profile.DTOs;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CareerFlow.Application.Features.Profile.Handlers;

public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, ProfileResponse>
{
    private readonly IPersonRepository _personRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProfileHandler> _logger;

    public UpdateProfileHandler(
        IPersonRepository personRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        ILogger<UpdateProfileHandler> logger)
    {
        _personRepository = personRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ProfileResponse> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();

        _logger.LogInformation("📝 Atualizando perfil do usuário: {UserId}", userId);

        var person = await _personRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Perfil", userId);

        person.UpdatePersonalInfo(
            command.Phone,
            command.City,
            command.State,
            command.BirthDate,
            command.ProfessionalSummary);
        person.UpdateCurrentProfession(command.CurrentPosition, command.CurrentCompany);

        _personRepository.Update(person);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ Perfil atualizado: {PersonId}", person.Id);

        return MapToResponse(person);
    }

    private static ProfileResponse MapToResponse(Person person)
    {
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
