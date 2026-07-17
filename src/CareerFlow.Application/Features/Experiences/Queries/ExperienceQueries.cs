using MediatR;
using CareerFlow.Application.Features.Experiences.DTOs;

namespace CareerFlow.Application.Features.Experiences.Queries;

public record GetExperiencesQuery : IRequest<IReadOnlyList<ExperienceResponse>>;
public record GetExperienceDetailQuery(Guid Id) : IRequest<ExperienceResponse>;
