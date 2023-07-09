using System.Reflection;
using MediatR;
using Stockband.Application.Common.Attributes;
using Stockband.Application.Interfaces.Services;
using Stockband.Domain.Common;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Behaviors;

public class AllowRoleBehavior<TRequest, TResponse>:IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuthenticationUserService _authenticationUserService;

    public AllowRoleBehavior(
        IAuthenticationUserService authenticationUserService)
    {
        _authenticationUserService = authenticationUserService;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        IEnumerable<AllowRoleAttribute> authorizedAttributes = request
            .GetType()
            .GetCustomAttributes<AllowRoleAttribute>()
            .ToList();

        if (!authorizedAttributes.Any())
        {
            return await next();
        }

        List<string> currentUserRoles = _authenticationUserService
            .GetCurrentUserRoles().ToList();

        UserRole[]? attributeRoles = authorizedAttributes.Select(x => x.Roles).FirstOrDefault();
        if (attributeRoles == null)
        {
            throw new ObjectNotFound(typeof(UserRole[]));
        }

        foreach (UserRole role in attributeRoles)
        {
            if (IsAuthorized(role, currentUserRoles))
            {
                return await next();
            }
        }
        
        return (TResponse)Activator.CreateInstance(typeof(TResponse), 
            new UnauthorizedOperationException(), 
            BaseErrorCode.UserUnauthorizedOperation)!;
    }

    private static bool IsAuthorized(UserRole userRole, List<string> userRoles)
    {
       return  userRoles.Contains(userRole.ToString());
    }
}