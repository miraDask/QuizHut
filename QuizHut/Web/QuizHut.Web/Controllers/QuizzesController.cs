namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Cache;
    using QuizHut.Services.Quiz;
    using QuizHut.Services.QuizResult;
    using QuizHut.Web.Controllers.Common;
    using QuizHut.Web.ViewModels.Quiz;

    public class QuizzesController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuizService quizService;
        private readonly IQuizResultService quizResultService;

        public QuizzesController(UserManager<ApplicationUser> userManager, IQuizService quizService, ICacheService cacheService, IQuizResultService quizResultService)
        {
            this.userManager = userManager;
            this.quizService = quizService;
            this.quizResultService = quizResultService;
        }

        public IActionResult DetailsInput()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AddDetails(InputQuizViewModel model)
        {
            var userId = this.userManager.GetUserId(this.User);
            model.CreatorId = userId;
            var quizId = await this.quizService.AddNewQuizAsync(model.Name, model.Description, model.ActivationDate, model.Duration, model.CreatorId);
            this.HttpContext.Session.SetString(Constants.QuizSeesionId, quizId);
            return this.RedirectToAction("QuestionInput", "Questions");
        }

        [HttpGet]
        public async Task<IActionResult> Display(string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                id = this.HttpContext.Session.GetString(Constants.QuizSeesionId);
            }

            var quizModel = await this.quizService.GetQuizByIdAsync<InputQuizViewModel>(id);

            return this.View(quizModel);
        }

        [HttpGet]
        public async Task<IActionResult> DisplayResult()
        {
            var userId = this.userManager.GetUserId(this.User);

            var model = new QuizResultViewModel()
            {
                QuizName = this.HttpContext.Session.GetString(Constants.AttemptedQuizName),
                MaxPoints = (int)this.HttpContext.Session.GetInt32(Constants.QuestionsCount),
                Points = (int)this.HttpContext.Session.GetInt32(Constants.QuizResult),
            };

            if (userId != null)
            {
                var quizId = this.HttpContext.Session.GetString(Constants.AttemptedQuizId);
                await this.quizResultService.SaveQuizResult(userId, quizId, model.Points, model.MaxPoints);
            }

            this.HttpContext.Session.Clear();

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Start(string id)
        {
            this.HttpContext.Session.SetString(Constants.AttemptedQuizId, id);
            var quizModel = await this.quizService.GetQuizByIdAsync<InputQuizViewModel>(id);
            this.HttpContext.Session.SetInt32(Constants.QuestionsCount, quizModel.Questions.Count);
            this.HttpContext.Session.SetString(Constants.AttemptedQuizName, quizModel.Name);

            return this.View(quizModel);
        }

        //[HttpPost]
        //public IActionResult Create(InputQuizViewModel quizModel)
        //{
        //    { 
        //    }
        //    return this.View(quizModel);
        //}
    }
}
