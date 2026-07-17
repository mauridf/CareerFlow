using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Features.Profile.DTOs;
using CareerFlow.Application.Features.Profile.Queries;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;
using CareerFlow.Core.Specifications;

namespace CareerFlow.Application.Features.Profile.Handlers;

public class GetProfileCompletionHandler : IRequestHandler<GetProfileCompletionQuery, ProfileCompletionResponse>
{
    private readonly IPersonRepository _personRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetProfileCompletionHandler> _logger;

    public GetProfileCompletionHandler(
        IPersonRepository personRepository,
        ICurrentUserService currentUser,
        ILogger<GetProfileCompletionHandler> logger)
    {
        _personRepository = personRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ProfileCompletionResponse> Handle(GetProfileCompletionQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();

        var person = await _personRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Perfil");

        var spec = new CompleteProfileSpecification(80);
        var missingFields = spec.GetMissingFields(person);

        var allFields = new List<string> { "Telefone", "Cidade", "Estado", "Data de nascimento",
            "Resumo profissional", "Foto de perfil", "Cargo atual", "Empresa atual" };

        var completedFields = allFields.Except(missingFields).ToList();

        return new ProfileCompletionResponse
        {
            Percentage = person.CalculateCompletionPercentage(),
            CanGenerateResume = person.CanGenerateResume(),
            MissingFields = missingFields,
            CompletedFields = completedFields
        };
    }
}
