using MediatR;
using CareerFlow.Application.Features.Languages.DTOs;

namespace CareerFlow.Application.Features.Languages.Queries;

public record GetLanguagesQuery : IRequest<IReadOnlyList<LanguageResponse>>;
