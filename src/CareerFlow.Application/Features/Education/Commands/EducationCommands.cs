using FluentValidation;
using MediatR;
using CareerFlow.Application.Features.Education.DTOs;
using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.Education.Commands;

public record CreateEducationCommand(
    string Institution,
    string Course,
    EducationLevel EducationLevel,
    DateTime StartDate,
    DateTime? EndDate,
    EducationStatus? Status,
    string? Description,
    string? Grade,
    string? ThesisTitle) : IRequest<EducationResponse>;

public class CreateEducationValidator : AbstractValidator<CreateEducationCommand>
{
    public CreateEducationValidator()
    {
        RuleFor(x => x.Institution).NotEmpty().WithMessage("Instituição é obrigatória");
        RuleFor(x => x.Course).NotEmpty().WithMessage("Curso é obrigatório");
        RuleFor(x => x.EducationLevel).IsInEnum().WithMessage("Nível de formação inválido");
    }
}

public record UpdateEducationCommand(
    Guid Id,
    string Institution,
    string Course,
    EducationLevel EducationLevel,
    DateTime StartDate,
    DateTime? EndDate,
    EducationStatus Status,
    string? Description,
    string? Grade,
    string? ThesisTitle) : IRequest<EducationResponse>;

public record DeleteEducationCommand(Guid Id) : IRequest;
