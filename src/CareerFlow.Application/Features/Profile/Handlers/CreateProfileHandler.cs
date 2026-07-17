using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Features.Profile.Commands;
using CareerFlow.Application.Features.Profile.DTOs;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Profile.Handlers;

public class CreateProfileHandler : IRequestHandler<CreateProfileCommand, ProfileResponse>
{
    private readonly IPersonRepository _personRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProfileHandler> _logger;

    public CreateProfileHandler(
        IPersonRepository personRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        ILogger<CreateProfileHandler> logger)
    {
        _personRepository = personRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ProfileResponse> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();

        _logger.LogInformation("📝 Criando perfil para usuário: {UserId}", userId);

        // Verifica se já existe perfil
        var existingPerson = await _personRepository.GetByUserIdAsync(userId, cancellationToken);
        if (existingPerson != null)
        {
            throw new ConflictException("Perfil", "userId", userId.ToString());
        }

        // Cria perfil
        var person = Person.Create(userId);
        person.UpdatePersonalInfo(
            command.Phone,
            command.City,
            command.State,
            command.BirthDate,
            command.ProfessionalSummary);
        person.UpdateCurrentProfession(command.CurrentPosition, command.CurrentCompany);

        // Gera slug baseado no nome do usuário
        var userName = _currentUser.Name ?? "user";
        person.SetResumeSlug(person.GenerateSlug(userName));

        await _personRepository.AddAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ Perfil criado: {PersonId}", person.Id);

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
