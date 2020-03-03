namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Quizzes;
    using QuizHut.Services.QuizzesResults;
    using QuizHut.Web.Common;
    using QuizHut.Web.ViewModels.Quizzes;

    public class QuizzesController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuizzesService quizService;
        private readonly IQuizzesResultsService quizResultService;

        public QuizzesController(
            UserManager<ApplicationUser> userManager,
            IQuizzesService quizService,
            IQuizzesResultsService quizResultService)
        {
            this.userManager = userManager;
            this.quizService = quizService;
            this.quizResultService = quizResultService;
        }

        public async Task<IActionResult> Start(string password)
        {
            var id = await this.quizService.GetQuizIdByPasswordAsync(password);
            if (id == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            this.HttpContext.Session.SetString(Constants.AttemptedQuizId, id);
            var quizModel = await this.quizService.GetQuizByIdAsync<AttemtedQuizViewModel>(id);
            this.HttpContext.Session.SetInt32(Constants.QuestionsCount, quizModel.Questions.Count);
            this.HttpContext.Session.SetString(Constants.AttemptedQuizName, quizModel.Name);

            return this.View(quizModel);
        }

        [HttpPost]
        public async Task<IActionResult> Start(PasswordInputViewModel model)
        {
            var id = await this.quizService.GetQuizIdByPasswordAsync(model.Password);
            if (id == null)
            {
                return this.RedirectToAction("Index", "Home", new { password = model.Password });
            }

            return this.RedirectToAction("Start", new { password = model.Password });
        }

        [HttpGet]
        public IActionResult Submit(QuizResultViewModel model)
        {
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(AttemtedQuizViewModel model)
        {
            var userId = this.userManager.GetUserId(this.User);
            var quizId = this.HttpContext.Session.GetString(Constants.AttemptedQuizId);
            var originalQuizModel = await this.quizService.GetQuizByIdAsync<InputQuizViewModel>(quizId);
            var resultModel = await this.quizResultService.GetResultModel(quizId, userId, originalQuizModel.Questions, model.Questions);
            resultModel.QuizName = this.HttpContext.Session.GetString(Constants.AttemptedQuizName);

            return this.View(resultModel);
        }
    }
}
