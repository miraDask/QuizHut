namespace QuizHut.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Web.ViewModels.Quizzes;

    public class HomeController : AdministrationController
    {
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
