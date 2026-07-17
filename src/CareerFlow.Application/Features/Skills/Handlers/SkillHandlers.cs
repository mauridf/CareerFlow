using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Features.Skills.Commands;
using CareerFlow.Application.Features.Skills.DTOs;
using CareerFlow.Application.Features.Skills.Queries;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Skills.Handlers;

public class CreateSkillHandler : IRequestHandler<CreateSkillCommand, SkillResponse>
{
    private readonly ISkillRepository _skillRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSkillHandler> _logger;

    public CreateSkillHandler(
        ISkillRepository skillRepository,
        ICurrentUserService currentUser,
        IPersonRepository personRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateSkillHandler> logger)
    {
        _skillRepository = skillRepository;
        _currentUser = currentUser;
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<SkillResponse> Handle(CreateSkillCommand command, CancellationToken cancellationToken)
    {
        var personId = await _currentUser.GetPersonIdAsync(cancellationToken);

        // Verifica limite de habilidades
        var count = await _skillRepository.CountAsync(s => s.PersonId == personId, cancellationToken);
        if (count >= 50)
            throw new DomainException("Limite máximo de 50 habilidades atingido");

        // Verifica duplicidade
        if (await _skillRepository.ExistsByNameAsync(personId, command.Name, cancellationToken))
            throw new ConflictException("Habilidade", "nome", command.Name);

        var skill = Skill.Create(personId, command.Name, command.Category, command.ProficiencyLevel, command.IsPrimary, command.DisplayOrder);

        await _skillRepository.AddAsync(skill, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ Habilidade criada: {SkillName} para PersonId={PersonId}", skill.Name, personId);

        return MapToResponse(skill);
    }

    private static SkillResponse MapToResponse(Skill skill) => new()
    {
        Id = skill.Id,
        Name = skill.Name,
        Category = skill.Category.GetDisplayName(),
        ProficiencyLevel = skill.ProficiencyLevel.GetDisplayName(),
        ProficiencyScore = skill.ProficiencyLevel.GetScore(),
        IsPrimary = skill.IsPrimary,
        DisplayOrder = skill.DisplayOrder,
        CreatedAt = skill.CreatedAt
    };
}

public class UpdateSkillHandler : IRequestHandler<UpdateSkillCommand, SkillResponse>
{
    private readonly ISkillRepository _skillRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSkillHandler> _logger;

    public UpdateSkillHandler(
        ISkillRepository skillRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        ILogger<UpdateSkillHandler> logger)
    {
        _skillRepository = skillRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<SkillResponse> Handle(UpdateSkillCommand command, CancellationToken cancellationToken)
    {
        var personId = await _currentUser.GetPersonIdAsync(cancellationToken);

        var skill = await _skillRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException("Habilidade", command.Id);

        if (skill.PersonId != personId)
            throw new UnauthorizedException("Esta habilidade não pertence ao seu perfil");

        skill.Update(command.Name, command.Category, command.ProficiencyLevel, command.IsPrimary, command.DisplayOrder);

        _skillRepository.Update(skill);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ Habilidade atualizada: {SkillId}", skill.Id);

        return new SkillResponse
        {
            Id = skill.Id,
            Name = skill.Name,
            Category = skill.Category.GetDisplayName(),
            ProficiencyLevel = skill.ProficiencyLevel.GetDisplayName(),
            ProficiencyScore = skill.ProficiencyLevel.GetScore(),
            IsPrimary = skill.IsPrimary,
            DisplayOrder = skill.DisplayOrder,
            CreatedAt = skill.CreatedAt
        };
    }
}

public class DeleteSkillHandler : IRequestHandler<DeleteSkillCommand>
{
    private readonly ISkillRepository _skillRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSkillHandler(
        ISkillRepository skillRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _skillRepository = skillRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteSkillCommand command, CancellationToken cancellationToken)
    {
        var personId = await _currentUser.GetPersonIdAsync(cancellationToken);

        var skill = await _skillRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException("Habilidade", command.Id);

        if (skill.PersonId != personId)
            throw new UnauthorizedException("Esta habilidade não pertence ao seu perfil");

        _skillRepository.Delete(skill);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public class GetSkillsHandler : IRequestHandler<GetSkillsQuery, IReadOnlyList<SkillResponse>>
{
    private readonly ISkillRepository _skillRepository;
    private readonly ICurrentUserService _currentUser;

    public GetSkillsHandler(ISkillRepository skillRepository, ICurrentUserService currentUser)
    {
        _skillRepository = skillRepository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<SkillResponse>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
    {
        var personId = await _currentUser.GetPersonIdAsync(cancellationToken);
        var skills = await _skillRepository.GetByPersonIdAsync(personId, cancellationToken);

        return skills.Select(s => new SkillResponse
        {
            Id = s.Id,
            Name = s.Name,
            Category = s.Category.GetDisplayName(),
            ProficiencyLevel = s.ProficiencyLevel.GetDisplayName(),
            ProficiencyScore = s.ProficiencyLevel.GetScore(),
            IsPrimary = s.IsPrimary,
            DisplayOrder = s.DisplayOrder,
            CreatedAt = s.CreatedAt
        }).ToList().AsReadOnly();
    }
}

public class GetSkillCategoriesHandler : IRequestHandler<GetSkillCategoriesQuery, IReadOnlyList<SkillCategoryResponse>>
{
    public Task<IReadOnlyList<SkillCategoryResponse>> Handle(GetSkillCategoriesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<SkillCategoryResponse> categories = Enum.GetValues<SkillCategory>()
            .Select(c => new SkillCategoryResponse
            {
                Value = c,
                DisplayName = c.GetDisplayName(),
                Icon = c.GetIcon()
            }).ToList().AsReadOnly();

        return Task.FromResult(categories);
    }
}

public class ReorderSkillsHandler : IRequestHandler<ReorderSkillsCommand>
{
    private readonly ISkillRepository _skillRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ReorderSkillsHandler(
        ISkillRepository skillRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _skillRepository = skillRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReorderSkillsCommand command, CancellationToken cancellationToken)
    {
        var personId = await _currentUser.GetPersonIdAsync(cancellationToken);
        var skills = await _skillRepository.GetByPersonIdAsync(personId, cancellationToken);
        var skillMap = skills.ToDictionary(s => s.Id);

        foreach (var item in command.Skills)
        {
            if (skillMap.TryGetValue(item.Id, out var skill))
            {
                skill.Update(skill.Name, skill.Category, skill.ProficiencyLevel, skill.IsPrimary, item.DisplayOrder);
                _skillRepository.Update(skill);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
