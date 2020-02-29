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
        public IActionResult Error(string code)
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier , StatusCode = code });
        }

        public IActionResult StatusCode(string code)
        {
            return this.RedirectToAction("Error", new { code });
        }
    }
}
