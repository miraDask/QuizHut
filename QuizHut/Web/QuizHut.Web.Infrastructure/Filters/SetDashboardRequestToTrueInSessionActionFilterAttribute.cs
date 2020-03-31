namespace QuizHut.Web.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using QuizHut.Common;

    public class SetDashboardRequestToTrueInSessionActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = (Controller)context.Controller;

            controller.HttpContext.Session.SetString(GlobalConstants.DashboardRequest, "true");
        }
    }
}
