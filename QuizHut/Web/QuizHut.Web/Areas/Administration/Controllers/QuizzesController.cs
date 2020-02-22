namespace QuizHut.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Quiz;
    using QuizHut.Web.ViewModels.Quizzes;
    using System.Linq;
    using System.Threading.Tasks;

    public class QuizzesController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuizService quizService;

        public QuizzesController(UserManager<ApplicationUser> userManager, IQuizService quizService)
        {
            this.userManager = userManager;
            this.quizService = quizService;
        }

        public async Task<IActionResult> AllQuizzesCreatedByUser()
        {
            var userId = this.userManager.GetUserId(this.User);
            var quizzes = await this.quizService.GetAllByCreatorIdAsync<QuizListViewModel>(userId);
            var model = new QuizzesAllListingViewModel() { Quizzes = quizzes };
            return this.View(model);
        }

        [HttpPost]
        public IActionResult AllQuizzesCreatedByUser(QuizzesAllListingViewModel model)
        {
            return this.View(model);
        }

    }
}