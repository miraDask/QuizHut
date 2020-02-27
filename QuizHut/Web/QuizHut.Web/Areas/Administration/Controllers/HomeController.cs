namespace QuizHut.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Web.ViewModels.Quizzes;

    public class HomeController : AdministrationController
    {
        public IActionResult Index(string password)
        {
            if (password != null)
            {
                return this.View(new PasswordInputViewModel() { Password = password });
            }

            return this.View();
        }
    }
}