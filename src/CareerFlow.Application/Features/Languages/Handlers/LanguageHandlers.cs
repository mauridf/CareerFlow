using MediatR;
using CareerFlow.Application.Features.Languages.Commands;
using CareerFlow.Application.Features.Languages.DTOs;
using CareerFlow.Application.Features.Languages.Queries;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Languages.Handlers;

public class CreateLanguageHandler : IRequestHandler<CreateLanguageCommand, LanguageResponse>
{
    private readonly ILanguageRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLanguageHandler(ILanguageRepository repo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _repo = repo; _currentUser = currentUser; _unitOfWork = unitOfWork;
    }

    public async Task<LanguageResponse> Handle(CreateLanguageCommand cmd, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        if (await _repo.CountByPersonIdAsync(personId, ct) >= 5)
            throw new DomainException("Limite máximo de 5 idiomas atingido");
        if (await _repo.ExistsByNameAsync(personId, cmd.LanguageName, ct))
            throw new ConflictException("Idioma", "nome", cmd.LanguageName);

        var lang = Language.Create(personId, cmd.LanguageName, cmd.ProficiencyLevel, cmd.IsNative);
        await _repo.AddAsync(lang, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new LanguageResponse
        {
            Id = lang.Id,
            LanguageName = lang.LanguageName,
            ProficiencyLevel = lang.ProficiencyLevel.GetDisplayName(),
            ProficiencyScore = lang.ProficiencyLevel.GetScore(),
            IsNative = lang.IsNative,
            DisplayOrder = lang.DisplayOrder,
            CreatedAt = lang.CreatedAt
        };
    }
}

public class GetLanguagesHandler : IRequestHandler<GetLanguagesQuery, IReadOnlyList<LanguageResponse>>
{
    private readonly ILanguageRepository _repo;
    private readonly ICurrentUserService _currentUser;

    public GetLanguagesHandler(ILanguageRepository repo, ICurrentUserService currentUser)
    {
        _repo = repo; _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<LanguageResponse>> Handle(GetLanguagesQuery req, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var langs = await _repo.GetByPersonIdAsync(personId, ct);
        return langs.Select(l => new LanguageResponse
        {
            Id = l.Id,
            LanguageName = l.LanguageName,
            ProficiencyLevel = l.ProficiencyLevel.GetDisplayName(),
            ProficiencyScore = l.ProficiencyLevel.GetScore(),
            IsNative = l.IsNative,
            DisplayOrder = l.DisplayOrder,
            CreatedAt = l.CreatedAt
        }).ToList().AsReadOnly();
    }
}

public class DeleteLanguageHandler : IRequestHandler<DeleteLanguageCommand>
{
    private readonly ILanguageRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLanguageHandler(ILanguageRepository repo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _repo = repo; _currentUser = currentUser; _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteLanguageCommand cmd, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var lang = await _repo.GetByIdAsync(cmd.Id, ct) ?? throw new NotFoundException("Idioma", cmd.Id);
        if (lang.PersonId != personId) throw new UnauthorizedException();
        _repo.Delete(lang);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}

public class UpdateLanguageHandler : IRequestHandler<UpdateLanguageCommand, LanguageResponse>
{
    private readonly ILanguageRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLanguageHandler(ILanguageRepository repo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _repo = repo; _currentUser = currentUser; _unitOfWork = unitOfWork;
    }

    public async Task<LanguageResponse> Handle(UpdateLanguageCommand cmd, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var lang = await _repo.GetByIdAsync(cmd.Id, ct) ?? throw new NotFoundException("Idioma", cmd.Id);
        if (lang.PersonId != personId) throw new UnauthorizedException();

        lang.Update(cmd.LanguageName, cmd.ProficiencyLevel, cmd.IsNative);

        _repo.Update(lang);
        await _unitOfWork.SaveChangesAsync(ct);

        return new LanguageResponse
        {
            Id = lang.Id,
            LanguageName = lang.LanguageName,
            ProficiencyLevel = lang.ProficiencyLevel.GetDisplayName(),
            ProficiencyScore = lang.ProficiencyLevel.GetScore(),
            IsNative = lang.IsNative,
            DisplayOrder = lang.DisplayOrder,
            CreatedAt = lang.CreatedAt
        };
    }
}
