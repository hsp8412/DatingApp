using API.Extensions;
using API.interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (!(resultContext.HttpContext.User?.Identity?.IsAuthenticated ?? false)) return;

        var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();

        if (repo == null) return;

        var userId = resultContext.HttpContext.User.GetUserId();

        if (userId == null) return;

        var user = await repo.GetUserByIdAsync(userId ?? 0);

        if (user == null) return;

        user.LastActive = DateTime.UtcNow;

        await repo.SaveAllAsync();
    }
}
