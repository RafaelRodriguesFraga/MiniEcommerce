using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CustomerService.Shared.Filters;

public class RequireUserIdAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId?.Value))
        {
            context.Result = new UnauthorizedResult();
        }

        context.HttpContext.Items["UserId"] = Guid.Parse(userId?.Value);

        base.OnActionExecuting(context);
    }
}