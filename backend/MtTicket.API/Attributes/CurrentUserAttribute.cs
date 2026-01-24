using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using MtTicket.API.Helpers;

namespace MtTicket.API.Attributes;

/// <summary>
/// Custom attribute để inject currentUserId vào action arguments nếu có.
/// </summary>
[Authorize]
public class CurrentUserAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        try
        {
            var userId = UserHelper.GetUserId(context.HttpContext.User);
            context.ActionArguments["currentUserId"] = userId;
        }
        catch
        {
            // Nếu không lấy được user, để pipeline xử lý authorize
        }

        base.OnActionExecuting(context);
    }
}

