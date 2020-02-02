namespace QuizHut.Web.Controllers
{
    using System.Security.Claims;
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
    using QuizHut.Web.ViewModels.Quiz;

    public class QuizController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuizService quizService;
        private readonly ICacheService cacheService;

        public QuizController(UserManager<ApplicationUser> userManager, IQuizService quizService, ICacheService cacheService)
        {
            this.userManager = userManager;
            this.quizService = quizService;
            this.cacheService = cacheService;
        }

        // GET: /<controller>/
        public IActionResult DetailsInput()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult AddDetails(InputQuizViewModel inputQuizViewModel)
        {
            var userId = this.userManager.GetUserId(this.User);
            inputQuizViewModel.CreatorId = userId;
            this.cacheService.SaveQuizModelToCache(inputQuizViewModel);

            return this.RedirectToAction("QuestionInput", "Question");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            return this.View();
        }

        [HttpGet]
        public string Display()
        {
            return JsonConvert.SerializeObject(this.cacheService.GetQuizModelFromCache());
        }
    }
}
