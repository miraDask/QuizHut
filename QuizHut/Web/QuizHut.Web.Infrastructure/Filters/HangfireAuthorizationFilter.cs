namespace QuizHut.Web.Infrastructure.Filters
{
    using Hangfire.Dashboard;
    using QuizHut.Common;

    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.Identity.IsAuthenticated
                && httpContext.User.IsInRole(GlobalConstants.AdministratorRoleName);
        }
    }
}
