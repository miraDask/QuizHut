namespace QuizHut.Web.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using QuizHut.Common;

    public class ChangeDefaoultLayoutActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var isAdministrator = context.HttpContext.User.IsInRole(GlobalConstants.AdministratorRoleName);
                var isTeacher = context.HttpContext.User.IsInRole(GlobalConstants.TeacherRoleName);

                var controller = (Controller)context.Controller;

                if (isAdministrator || isTeacher)
                {
                    controller.ViewData["Layout"] = GlobalConstants.AdminLayout;
                }
                else
                {
                    controller.ViewData["Layout"] = GlobalConstants.StudentLayout;
                }
            }
        }
    }
}
