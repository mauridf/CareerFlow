using MediatR;
using CareerFlow.Application.Features.Profile.DTOs;

namespace CareerFlow.Application.Features.Profile.Queries;

public record GetProfileQuery : IRequest<ProfileResponse>;
