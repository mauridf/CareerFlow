using MediatR;
using CareerFlow.Application.Features.Auth.DTOs;

namespace CareerFlow.Application.Features.Auth.Commands;

public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken) : IRequest<AuthResponse>;
