namespace QuizHut.Web.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Web.ViewModels;
    using QuizHut.Web.ViewModels.Quizzes;

    public class HomeController : BaseController
    {
        public IActionResult Index(string password)
        {
            if (password != null)
            {
                return this.View(new PasswordInputViewModel() { Password = password });
            }

            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
