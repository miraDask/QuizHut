namespace QuizHut.Web.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ClearDashboardRequestInSessionActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = (Controller)context.Controller;
            if (controller.HttpContext.Session.GetString("DashboardRequest") != null)
            {
                controller.HttpContext.Session.Clear();
            }
        }
    }
}
