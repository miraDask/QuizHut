namespace QuizHut.Web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Quiz;
    using QuizHut.Web.ViewModels.Quiz;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class QuizController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuizService quizService;

        public QuizController(UserManager<ApplicationUser> userManager, IQuizService quizService)
        {
            this.userManager = userManager;
            this.quizService = quizService;
        }

        // GET: /<controller>/
        public IActionResult NameInput()
        {
            return this.View();
        }

        public IActionResult ActivationDateInput()
        {
            return this.View();
        }

        public IActionResult DurationInput()
        {
            return this.View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(InputQuizViewModel model)
        //{
        //    var userId = this.userManager.GetUserId(this.User);
        //    model.CreatorId = userId;
        //    await this.quizService.AddNewQuizAsync<InputQuizViewModel>(model);
        //    return Json(model);
        //}

    }
}
