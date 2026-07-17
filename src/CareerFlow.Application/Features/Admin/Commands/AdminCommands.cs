using MediatR;
using CareerFlow.Application.Features.Admin.DTOs;

namespace CareerFlow.Application.Features.Admin.Commands;

public record AdminUpdateUserCommand(Guid Id, string? Name, string? Role) : IRequest<AdminUserDetailResponse>;
public record AdminToggleUserStatusCommand(Guid Id) : IRequest<AdminUserDetailResponse>;
public record AdminManagePremiumCommand(Guid Id, bool Activate, DateTime? Until = null) : IRequest<AdminUserDetailResponse>;
public record AdminDeleteUserCommand(Guid Id) : IRequest;
