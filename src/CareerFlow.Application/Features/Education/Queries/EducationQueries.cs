using MediatR;
using CareerFlow.Application.Features.Education.DTOs;

namespace CareerFlow.Application.Features.Education.Queries;

public record GetEducationQuery : IRequest<IReadOnlyList<EducationResponse>>;
