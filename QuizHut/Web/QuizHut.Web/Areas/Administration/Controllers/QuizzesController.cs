namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Quiz;
    using QuizHut.Web.Filters;
    using QuizHut.Web.ViewModels.Quizzes;

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
        [ModelStateValidationActionFilterAttribute]
        public IActionResult AllQuizzesCreatedByUser(QuizzesAllListingViewModel model)
        {
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> Start(PasswordInputViewModel model)
        {
            var id = await this.quizService.GetQuizIdByPasswordAsync(model.Password);
            if (id == null)
            {
                return this.RedirectToAction("Index", "Home", new { password = model.Password, area = "Administration" });
            }

            return this.RedirectToAction("Start", "Quizzes", new { area = string.Empty, password = model.Password });
        }
    }
}