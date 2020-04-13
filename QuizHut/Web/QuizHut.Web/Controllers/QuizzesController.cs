namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Data.Models;
    using QuizHut.Services.Questions;
    using QuizHut.Services.Quizzes;
    using QuizHut.Services.Results;
    using QuizHut.Web.Common;
    using QuizHut.Web.Infrastructure.Helpers;
    using QuizHut.Web.ViewModels.Questions;
    using QuizHut.Web.ViewModels.Quizzes;

    [Authorize]
    public class QuizzesController : Controller
    {
        private readonly IResultHelper resultHelper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuizzesService quizService;
        private readonly IResultsService resultService;
        private readonly IQuestionsService questionsService;

        public QuizzesController(
            IResultHelper resultHelper,
            UserManager<ApplicationUser> userManager,
            IQuizzesService quizService,
            IResultsService resultService,
            IQuestionsService questionsService)
        {
            this.resultHelper = resultHelper;
            this.userManager = userManager;
            this.quizService = quizService;
            this.resultService = resultService;
            this.questionsService = questionsService;
        }

        public async Task<IActionResult> Start(string password, string id)
        {
            id ??= await this.quizService.GetQuizIdByPasswordAsync(password);

            var user = await this.userManager.GetUserAsync(this.User);
            var roles = await this.userManager.GetRolesAsync(user);
            var userHasPermitionToTakeTheQuiz = await this.quizService.HasUserPermition(user.Id, id);

            if (!userHasPermitionToTakeTheQuiz)
            {
                var controller = roles.Count > 0 ? "Home" : "Students";
                var routObject = new
                {
                    password,
                    area = roles.Count > 0 ? GlobalConstants.Administration : string.Empty,
                    errorText = GlobalConstants.ErrorMessages.PermissionDenied,
                };

                return this.RedirectToAction("Index", controller, routObject);
            }

            this.ViewData["Area"] = roles.Count > 0 ? Constants.AdminArea : string.Empty;
            var quizModel = await this.quizService.GetQuizByIdAsync<AttemtedQuizViewModel>(id);
            return this.View(quizModel);
        }

        [HttpPost]
        public async Task<IActionResult> Start(PasswordInputViewModel model)
        {
            var id = await this.quizService.GetQuizIdByPasswordAsync(model.Password);

            if (model.Password == null || id == null)
            {
                var routObject = new
                {
                    password = model.Password,
                    area = string.Empty,
                    errorText = model.Password == null ? GlobalConstants.ErrorMessages.EmptyPasswordField
                        : id == null ? string.Format(GlobalConstants.ErrorMessages.QuizNotFound, model.Password) : null,
                };

                return this.RedirectToAction("Index", "Students", routObject);
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
            var originalQuestions = await this.questionsService.GetAllByQuizIdAsync<QuestionViewModel>(model.Id);
            var points = this.resultHelper.CalculateResult(originalQuestions, model.Questions);
            var creatorId = await this.quizService.GetCreatorIdByQuizIdAsync(model.Id);
            if (creatorId != userId)
            {
                await this.resultService.CreateResultAsync(userId, points, originalQuestions.Count, model.Id);
            }

            var resultModel = new QuizResultViewModel()
            {
                QuizName = model.Name,
                MaxPoints = originalQuestions.Count,
                Points = points,
            };

            return this.View(resultModel);
        }
    }
}
