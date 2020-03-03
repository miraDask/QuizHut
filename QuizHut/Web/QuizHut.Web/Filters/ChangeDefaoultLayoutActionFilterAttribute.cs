namespace QuizHut.Web.Filters
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using QuizHut.Common;
    using QuizHut.Data.Models;
    using QuizHut.Web.Common;

    public class ChangeDefaoultLayoutActionFilterAttribute : ActionFilterAttribute
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ChangeDefaoultLayoutActionFilterAttribute(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = this.userManager.GetUserId(context.HttpContext.User);
            var isAdministrator = context.HttpContext.User.IsInRole(GlobalConstants.AdministratorRoleName);
            var isTeacher = context.HttpContext.User.IsInRole(GlobalConstants.TeacherRoleName);

            if (userId != null && (isAdministrator || isTeacher))
            {
                var controller = (Controller)context.Controller;

                controller.ViewData["Layout"] = Constants.AdminLayout;
            }
        }
    }
}
