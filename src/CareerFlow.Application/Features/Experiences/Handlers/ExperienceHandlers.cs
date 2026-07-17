using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Features.Experiences.Commands;
using CareerFlow.Application.Features.Experiences.DTOs;
using CareerFlow.Application.Features.Experiences.Queries;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Experiences.Handlers;

public class CreateExperienceHandler : IRequestHandler<CreateExperienceCommand, ExperienceResponse>
{
    private readonly IExperienceRepository _experienceRepository;
    private readonly ISkillRepository _skillRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateExperienceHandler> _logger;

    public CreateExperienceHandler(
        IExperienceRepository experienceRepository,
        ISkillRepository skillRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        ILogger<CreateExperienceHandler> logger)
    {
        _experienceRepository = experienceRepository;
        _skillRepository = skillRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ExperienceResponse> Handle(CreateExperienceCommand command, CancellationToken cancellationToken)
    {
        var personId = await _currentUser.GetPersonIdAsync(cancellationToken);

        var count = await _experienceRepository.CountByPersonIdAsync(personId, cancellationToken);
        if (count >= 20)
            throw new DomainException("Limite máximo de 20 experiências atingido");

        // Valida skills se fornecidas
        if (command.SkillsUsed?.Count > 0)
        {
            var personSkills = await _skillRepository.GetByPersonIdAsync(personId, cancellationToken);
            var validSkillIds = personSkills.Select(s => s.Id).ToHashSet();

            var invalidSkills = command.SkillsUsed.Where(id => !validSkillIds.Contains(id)).ToList();
            if (invalidSkills.Count > 0)
                throw new ValidationException("skillsUsed", $"Habilidades inválidas: {string.Join(", ", invalidSkills)}");
        }

        var experience = Experience.Create(
            personId, command.CompanyName, command.Position,
            command.StartDate, command.EndDate, command.Description,
            command.SkillsUsed, command.City, command.State, command.Country,
            command.EmploymentType);

        await _experienceRepository.AddAsync(experience, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ Experiência criada: {Company} - {Position}", experience.CompanyName, experience.Position);

        return MapToResponse(experience);
    }

    private static ExperienceResponse MapToResponse(Experience exp) => new()
    {
        Id = exp.Id,
        CompanyName = exp.CompanyName,
        Position = exp.Position,
        StartDate = exp.StartDate,
        EndDate = exp.EndDate,
        IsCurrent = exp.IsCurrent,
        Description = exp.Description,
        SkillsUsed = exp.SkillsUsed,
        City = exp.City,
        State = exp.State,
        Country = exp.Country,
        EmploymentType = exp.EmploymentType?.GetDisplayName(),
        DurationFormatted = exp.GetFormattedDuration(),
        DisplayOrder = exp.DisplayOrder,
        CreatedAt = exp.CreatedAt
    };
}

public class GetExperiencesHandler : IRequestHandler<GetExperiencesQuery, IReadOnlyList<ExperienceResponse>>
{
    private readonly IExperienceRepository _experienceRepository;
    private readonly ICurrentUserService _currentUser;

    public GetExperiencesHandler(IExperienceRepository experienceRepository, ICurrentUserService currentUser)
    {
        _experienceRepository = experienceRepository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ExperienceResponse>> Handle(GetExperiencesQuery request, CancellationToken cancellationToken)
    {
        var personId = await _currentUser.GetPersonIdAsync(cancellationToken);
        var experiences = await _experienceRepository.GetByPersonIdAsync(personId, cancellationToken);

        return experiences.Select(exp => new ExperienceResponse
        {
            Id = exp.Id,
            CompanyName = exp.CompanyName,
            Position = exp.Position,
            StartDate = exp.StartDate,
            EndDate = exp.EndDate,
            IsCurrent = exp.IsCurrent,
            Description = exp.Description,
            SkillsUsed = exp.SkillsUsed,
            City = exp.City,
            State = exp.State,
            Country = exp.Country,
            EmploymentType = exp.EmploymentType?.GetDisplayName(),
            DurationFormatted = exp.GetFormattedDuration(),
            DisplayOrder = exp.DisplayOrder,
            CreatedAt = exp.CreatedAt
        }).ToList().AsReadOnly();
    }
}

public class DeleteExperienceHandler : IRequestHandler<DeleteExperienceCommand>
{
    private readonly IExperienceRepository _experienceRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExperienceHandler(
        IExperienceRepository experienceRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _experienceRepository = experienceRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteExperienceCommand command, CancellationToken cancellationToken)
    {
        var personId = await _currentUser.GetPersonIdAsync(cancellationToken);

        var experience = await _experienceRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException("Experiência", command.Id);

        if (experience.PersonId != personId)
            throw new UnauthorizedException("Esta experiência não pertence ao seu perfil");

        _experienceRepository.Delete(experience);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
