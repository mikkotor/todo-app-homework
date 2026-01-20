using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TodoApi.Filters;

/// <summary>
/// Ensures an authenticated request contains an Auth0 user id.
/// If missing, short-circuits with 401. Otherwise places the id in
/// <see cref="HttpContext.Items"/> under <see cref="UserId"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthenticatedAttribute : Attribute, IAsyncActionFilter
{
    public const string UserId = "UserId";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var http = context.HttpContext;

        var id = http.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(id))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        http.Items[UserId] = id;
        await next();
    }
}
