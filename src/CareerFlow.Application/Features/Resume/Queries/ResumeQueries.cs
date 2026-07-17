using MediatR;
using CareerFlow.Application.Features.Resume.DTOs;

namespace CareerFlow.Application.Features.Resume.Queries;

public record GetResumeQuery : IRequest<ResumeResponse>;
public record GetPublicResumeQuery(string Slug) : IRequest<ResumeResponse>;
public record GetResumeAnalyticsQuery : IRequest<ResumeAnalyticsResponse>;
public record GetResumeSuggestionsQuery : IRequest<IReadOnlyList<SuggestionDto>>;
