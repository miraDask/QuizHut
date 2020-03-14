namespace QuizHut.Web.Filters
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using QuizHut.Common;
    using QuizHut.Web.Common;

    public class OverrideDefoultLayoutPageFilter : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var isAdministrator = context.HttpContext.User.IsInRole(GlobalConstants.AdministratorRoleName);
                var isTeacher = context.HttpContext.User.IsInRole(GlobalConstants.TeacherRoleName);

                var result = context.Result as PageResult;

                if (isAdministrator || isTeacher)
                {
                    result.ViewData["Layout"] = Constants.AdminLayout;
                }
                else
                {
                    result.ViewData["Layout"] = Constants.StudentLayout;
                }
            }
        }
    }
}
