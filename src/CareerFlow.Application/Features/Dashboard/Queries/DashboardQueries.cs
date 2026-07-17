using MediatR;
using CareerFlow.Application.Features.Dashboard.DTOs;

namespace CareerFlow.Application.Features.Dashboard.Queries;

public record GetDashboardStatsQuery : IRequest<DashboardStatsResponse>;
public record GetResumeInsightsQuery : IRequest<ResumeInsightsResponse>;
public record GetRecentActivityQuery(int Limit = 10) : IRequest<IReadOnlyList<ActivityResponse>>;
public record GetViewsChartQuery(int Days = 30) : IRequest<ViewsChartResponse>;
public record GetSkillsGapQuery : IRequest<SkillsGapResponse>;
