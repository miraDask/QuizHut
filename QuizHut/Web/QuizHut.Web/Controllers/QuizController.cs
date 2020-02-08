namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Cache;
    using QuizHut.Services.Quiz;
    using QuizHut.Web.Controllers.Common;
    using QuizHut.Web.ViewModels.Quiz;
    using ReflectionIT.Mvc.Paging;

    public class QuizController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuizService quizService;

        public QuizController(UserManager<ApplicationUser> userManager, IQuizService quizService, ICacheService cacheService)
        {
            this.userManager = userManager;
            this.quizService = quizService;
        }

        public IActionResult DetailsInput()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AddDetails(InputQuizViewModel inputQuizViewModel)
        {
            var userId = this.userManager.GetUserId(this.User);
            inputQuizViewModel.CreatorId = userId;
            var quizId = await this.quizService.AddNewQuizAsync(inputQuizViewModel);
            this.HttpContext.Session.SetString(Constants.QuizSeesionId, quizId);
            return this.RedirectToAction("QuestionInput", "Question");
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

        //[HttpPost]
        //public async Task<IActionResult> Display(string id)
        //{
        //    var quizModel = await this.quizService.GetQuizByIdAsync<InputQuizViewModel>("8986f029-4f92-49e5-a4dc-01702e10c843");
        //    return this.View(quizModel);
        //}


        [HttpPost]
        public async Task<IActionResult> Start(string id)
        {
            this.HttpContext.Session.SetString(Constants.AttemptedQuizId, id);
            var quizModel = await this.quizService.GetQuizByIdAsync<InputQuizViewModel>(id);
            return this.View(quizModel);
        }


        //[HttpPost]
        //public IActionResult Create(InputQuizViewModel quizModel)
        //{
        //    return this.View(quizModel);
        //}
    }
}
