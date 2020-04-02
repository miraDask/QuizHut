namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Data.Models;
    using QuizHut.Services.Quizzes;
    using QuizHut.Services.Results;
    using QuizHut.Web.Common;
    using QuizHut.Web.Infrastructure.Helpers;
    using QuizHut.Web.ViewModels.Quizzes;

    [Authorize]
    public class QuizzesController : Controller
    {
        private readonly IResultHelper resultHelper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuizzesService quizService;
        private readonly IResultsService resultService;

        public QuizzesController(
            IResultHelper resultHelper,
            UserManager<ApplicationUser> userManager,
            IQuizzesService quizService,
            IResultsService resultService)
        {
            this.resultHelper = resultHelper;
            this.userManager = userManager;
            this.quizService = quizService;
            this.resultService = resultService;
        }

        public async Task<IActionResult> Start(string password, string id)
        {
            if (id == null)
            {
                id = await this.quizService.GetQuizIdByPasswordAsync(password);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            var userHasPermitionToTakeTheQuiz = await this.quizService.HasUserPermition(user.Id, id);
            var roles = await this.userManager.GetRolesAsync(user);

            if (!userHasPermitionToTakeTheQuiz)
            {
                if (roles.Count > 0)
                {
                    return this.RedirectToAction("Index", "Home", new
                    {
                        password,
                        area = GlobalConstants.Administration,
                        errorText = GlobalConstants.ErrorMessages.PermissionDenied,
                    });
                }
                else
                {
                    return this.RedirectToAction("Index", "Students", new
                    {
                        password,
                        area = string.Empty,
                        errorText = GlobalConstants.ErrorMessages.PermissionDenied,
                    });
                }
            }

            if (roles.Count > 0)
            {
                this.ViewData["Area"] = Constants.AdminArea;
            }

            var quizModel = await this.quizService.GetQuizByIdAsync<AttemtedQuizViewModel>(id);

            return this.View(quizModel);
        }

        [HttpPost]
        public async Task<IActionResult> Start(PasswordInputViewModel model)
        {
            if (model.Password == null)
            {
                return this.RedirectToAction("Index", "Students", new
                {
                    password = model.Password,
                    area = string.Empty,
                    errorText = GlobalConstants.ErrorMessages.EmptyPasswordField,
                });
            }

            var id = await this.quizService.GetQuizIdByPasswordAsync(model.Password);

            if (id == null)
            {
                return this.RedirectToAction("Index", "Students", new
                {
                    password = model.Password,
                    area = string.Empty,
                    errorText = string.Format(GlobalConstants.ErrorMessages.QuizNotFound, model.Password),
                });
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
            var originalQuizModel = await this.quizService.GetQuizByIdAsync<QuizDetailsViewModel>(model.Id);
            var points = this.resultHelper.CalculateResult(originalQuizModel.Questions, model.Questions);
            var creatorId = await this.quizService.GetCreatorIdByQuizIdAsync(model.Id);
            if (creatorId != userId)
            {
                await this.resultService.CreateResultAsync(userId, points, originalQuizModel.Questions.Count, model.Id);
            }

            var resultModel = new QuizResultViewModel()
            {
                QuizName = model.Name,
                MaxPoints = originalQuizModel.Questions.Count,
                Points = points,
            };

            return this.View(resultModel);
        }
    }
}
