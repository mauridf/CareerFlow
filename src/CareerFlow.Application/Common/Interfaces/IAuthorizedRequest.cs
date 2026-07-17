namespace CareerFlow.Application.Common.Interfaces;

public interface IAuthorizedRequest
{
    string? RequiredRole { get; }
    bool RequirePremium { get; }
}
