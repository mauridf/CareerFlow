using MediatR;
using CareerFlow.Application.Features.Skills.DTOs;

namespace CareerFlow.Application.Features.Skills.Queries;

public record GetSkillsQuery : IRequest<IReadOnlyList<SkillResponse>>;
public record GetSkillCategoriesQuery : IRequest<IReadOnlyList<SkillCategoryResponse>>;
