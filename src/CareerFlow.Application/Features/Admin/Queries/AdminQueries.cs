using MediatR;
using CareerFlow.Application.Features.Admin.DTOs;

namespace CareerFlow.Application.Features.Admin.Queries;

public record GetSystemStatsQuery : IRequest<AdminSystemStatsResponse>;
public record GetUsersListQuery(int Page = 1, int PageSize = 20, string? Search = null) : IRequest<AdminUsersListResponse>;
public record GetUserDetailQuery(Guid Id) : IRequest<AdminUserDetailResponse>;
