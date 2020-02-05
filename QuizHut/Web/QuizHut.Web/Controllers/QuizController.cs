namespace QuizHut.Web.Controllers
{
    using System.Security.Claims;
    using System.Text;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Cache;
    using QuizHut.Services.Quiz;
    using QuizHut.Web.ViewModels.Question;
    using QuizHut.Web.ViewModels.Quiz;

    public class QuizController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuizService quizService;

        public QuizController(UserManager<ApplicationUser> userManager, IQuizService quizService, ICacheService cacheService)
        {
            this.userManager = userManager;
            this.quizService = quizService;
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(InputQuizViewModel model)
        //{
        //    { }

        //    return this.View();
        //}

        // GET: /<controller>/
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
            var questionModel = new QuestionViewModel() { QuizId = quizId };
            return this.RedirectToAction("QuestionInput", "Question", questionModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            return this.View();
        }

        [HttpGet]
        public async Task<IActionResult> Display()
        {
            var data = this.ViewData["quizId"];
            //var model = await this.cacheService.GetQuizModelFromCacheAsync();
            return this.View();
        }
    }
}
