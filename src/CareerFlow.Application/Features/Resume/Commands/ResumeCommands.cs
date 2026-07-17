using MediatR;
using CareerFlow.Application.Features.Resume.DTOs;

namespace CareerFlow.Application.Features.Resume.Commands;

public record GenerateResumeCommand : IRequest<byte[]>;
public record GenerateAtsResumeCommand : IRequest<byte[]>;
public record ShareResumeCommand : IRequest<string>;
public record AnalyzeResumeCommand : IRequest<ResumeAnalyticsResponse>;
public record PublishResumeCommand : IRequest;
public record UnpublishResumeCommand : IRequest;
public record GenerateResumeSuggestionsCommand : IRequest<IReadOnlyList<SuggestionDto>>;
