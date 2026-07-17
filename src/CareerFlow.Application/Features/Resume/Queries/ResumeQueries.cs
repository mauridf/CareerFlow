using MediatR;
using CareerFlow.Application.Features.Resume.DTOs;

namespace CareerFlow.Application.Features.Resume.Queries;

public record GetResumeQuery : IRequest<ResumeResponse>;
public record GetPublicResumeQuery(string Slug) : IRequest<ResumeResponse>;
public record GetResumeAnalyticsQuery : IRequest<ResumeAnalyticsResponse>;
public record GetResumeSuggestionsQuery : IRequest<IReadOnlyList<SuggestionDto>>;
public record GetPublicResumePdfQuery(string Slug) : IRequest<byte[]>;
public record GetPublicAtsResumePdfQuery(string Slug) : IRequest<byte[]>;
