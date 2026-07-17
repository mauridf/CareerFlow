using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Features.Education.Commands;
using CareerFlow.Application.Features.Education.DTOs;
using CareerFlow.Application.Features.Education.Queries;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;
using EducationEntity = CareerFlow.Core.Entities.Education;

namespace CareerFlow.Application.Features.Education.Handlers;

public class CreateEducationHandler : IRequestHandler<CreateEducationCommand, EducationResponse>
{
    private readonly IEducationRepository _educationRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateEducationHandler> _logger;

    public CreateEducationHandler(
        IEducationRepository educationRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        ILogger<CreateEducationHandler> logger)
    {
        _educationRepository = educationRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<EducationResponse> Handle(CreateEducationCommand command, CancellationToken cancellationToken)
    {
        var personId = await _currentUser.GetPersonIdAsync(cancellationToken);

        var count = await _educationRepository.CountByPersonIdAsync(personId, cancellationToken);
        if (count >= 10)
            throw new DomainException("Limite máximo de 10 formações atingido");

        var education = EducationEntity.Create(
            personId, command.Institution, command.Course, command.EducationLevel,
            command.StartDate, command.EndDate, command.Status,
            command.Description, command.Grade, command.ThesisTitle);

        await _educationRepository.AddAsync(education, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ Formação criada: {Institution} - {Course}", education.Institution, education.Course);

        return MapToResponse(education);
    }

    private static EducationResponse MapToResponse(EducationEntity edu) => new()
    {
        Id = edu.Id,
        Institution = edu.Institution,
        Course = edu.Course,
        EducationLevel = edu.EducationLevel.GetDisplayName(),
        Status = edu.Status.GetDisplayName(),
        StartDate = edu.StartDate,
        EndDate = edu.EndDate,
        IsCurrent = edu.IsCurrent,
        Description = edu.Description,
        Grade = edu.Grade,
        ThesisTitle = edu.ThesisTitle,
        DisplayOrder = edu.DisplayOrder,
        CreatedAt = edu.CreatedAt
    };
}

public class GetEducationHandler : IRequestHandler<GetEducationQuery, IReadOnlyList<EducationResponse>>
{
    private readonly IEducationRepository _educationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetEducationHandler(IEducationRepository educationRepository, ICurrentUserService currentUser)
    {
        _educationRepository = educationRepository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<EducationResponse>> Handle(GetEducationQuery request, CancellationToken cancellationToken)
    {
        var personId = await _currentUser.GetPersonIdAsync(cancellationToken);
        var educations = await _educationRepository.GetByPersonIdAsync(personId, cancellationToken);

        return educations.Select(edu => new EducationResponse
        {
            Id = edu.Id,
            Institution = edu.Institution,
            Course = edu.Course,
            EducationLevel = edu.EducationLevel.GetDisplayName(),
            Status = edu.Status.GetDisplayName(),
            StartDate = edu.StartDate,
            EndDate = edu.EndDate,
            IsCurrent = edu.IsCurrent,
            Description = edu.Description,
            Grade = edu.Grade,
            ThesisTitle = edu.ThesisTitle,
            DisplayOrder = edu.DisplayOrder,
            CreatedAt = edu.CreatedAt
        }).ToList().AsReadOnly();
    }
}

public class DeleteEducationHandler : IRequestHandler<DeleteEducationCommand>
{
    private readonly IEducationRepository _educationRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEducationHandler(
        IEducationRepository educationRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _educationRepository = educationRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteEducationCommand command, CancellationToken cancellationToken)
    {
        var personId = await _currentUser.GetPersonIdAsync(cancellationToken);

        var education = await _educationRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException("Formação", command.Id);

        if (education.PersonId != personId)
            throw new UnauthorizedException("Esta formação não pertence ao seu perfil");

        _educationRepository.Delete(education);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public class UpdateEducationHandler : IRequestHandler<UpdateEducationCommand, EducationResponse>
{
    private readonly IEducationRepository _educationRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateEducationHandler> _logger;

    public UpdateEducationHandler(
        IEducationRepository educationRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        ILogger<UpdateEducationHandler> logger)
    {
        _educationRepository = educationRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<EducationResponse> Handle(UpdateEducationCommand command, CancellationToken cancellationToken)
    {
        var personId = await _currentUser.GetPersonIdAsync(cancellationToken);

        var education = await _educationRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException("Formação", command.Id);

        if (education.PersonId != personId)
            throw new UnauthorizedException("Esta formação não pertence ao seu perfil");

        education.Update(
            command.Institution,
            command.Course,
            command.EducationLevel,
            command.StartDate,
            command.EndDate,
            command.Status,
            command.Description,
            command.Grade,
            command.ThesisTitle);

        _educationRepository.Update(education);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ Formação atualizada: {Id}", education.Id);

        return new EducationResponse
        {
            Id = education.Id,
            Institution = education.Institution,
            Course = education.Course,
            EducationLevel = education.EducationLevel.GetDisplayName(),
            Status = education.Status.GetDisplayName(),
            StartDate = education.StartDate,
            EndDate = education.EndDate,
            IsCurrent = education.IsCurrent,
            Description = education.Description,
            Grade = education.Grade,
            ThesisTitle = education.ThesisTitle,
            DisplayOrder = education.DisplayOrder,
            CreatedAt = education.CreatedAt
        };
    }
}
