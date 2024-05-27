using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class RequireOneRoleAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly string[] _allowedRoles;

    public RequireOneRoleAttribute(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
        if (_allowedRoles.Length == 0)
            throw new ArgumentException("At least one allowed role must be specified.", nameof(allowedRoles));
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context?.HttpContext?.User?.Claims == null)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }

        var userRoles = context.HttpContext.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);

        if (!userRoles.Any())
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }

        var hasRequiredRole = _allowedRoles.Any(role => userRoles.Contains(role));

        if (!hasRequiredRole)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            return;
        }
    }
}

