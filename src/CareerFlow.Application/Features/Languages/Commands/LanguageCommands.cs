using FluentValidation;
using MediatR;
using CareerFlow.Application.Features.Languages.DTOs;
using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.Languages.Commands;

public record CreateLanguageCommand(string LanguageName, LanguageLevel ProficiencyLevel, bool IsNative) : IRequest<LanguageResponse>;
public record UpdateLanguageCommand(Guid Id, string LanguageName, LanguageLevel ProficiencyLevel, bool IsNative) : IRequest<LanguageResponse>;
public record DeleteLanguageCommand(Guid Id) : IRequest;

public class CreateLanguageValidator : AbstractValidator<CreateLanguageCommand>
{
    public CreateLanguageValidator()
    {
        RuleFor(x => x.LanguageName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ProficiencyLevel).IsInEnum();
    }
}
