namespace QuizHut.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Web.Infrastructure.Filters;
    using QuizHut.Web.ViewModels.Quizzes;

    public class HomeController : AdministrationController
    {
        [ClearDashboardRequestInSessionActionFilterAttribute]
        public IActionResult Index(string password, string errorText)
        {
            var model = new PasswordInputViewModel();
            if (errorText != null)
            {
                model.Password = password;
                model.Error = errorText;
            }

            return this.View(model);
        }
    }
}
