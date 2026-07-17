using MediatR;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUser;

    public AuthorizationBehavior(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IAuthorizedRequest authRequest)
        {
            if (!_currentUser.IsAuthenticated)
                throw new UnauthorizedAccessException("Usuário não autenticado");

            if (authRequest.RequiredRole != null &&
                !string.Equals(_currentUser.Role, authRequest.RequiredRole, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException($"Acesso negado. Role '{authRequest.RequiredRole}' é necessária.");
            }

            if (authRequest.RequirePremium && !_currentUser.IsPremium)
            {
                throw new UnauthorizedAccessException("Acesso negado. Assinatura premium é necessária.");
            }
        }

        return await next();
    }
}
