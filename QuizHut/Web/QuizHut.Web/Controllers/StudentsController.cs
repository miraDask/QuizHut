namespace QuizHut.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Web.ViewModels.Quizzes;

    public class StudentsController : Controller
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