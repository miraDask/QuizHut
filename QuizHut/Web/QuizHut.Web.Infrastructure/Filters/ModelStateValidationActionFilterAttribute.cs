namespace QuizHut.Web.Infrastructure.Filters
{
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ModelStateValidationActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var model = context.ActionArguments.First().Value;
                var controller = (Controller)context.Controller;
                var actionName = (string)context.RouteData.Values["action"];
                context.Result = controller.View(actionName, model);
            }
        }
    }
}
