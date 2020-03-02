namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Quiz;
    using QuizHut.Services.QuizResult;
    using QuizHut.Web.Controllers.Common;
    using QuizHut.Web.Filters;
    using QuizHut.Web.ViewModels.Quizzes;
    using Rotativa.AspNetCore;

    public class QuizzesController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuizService quizService;
        private readonly IQuizResultService quizResultService;

        public QuizzesController(
            UserManager<ApplicationUser> userManager,
            IQuizService quizService,
            IQuizResultService quizResultService)
        {
            this.userManager = userManager;
            this.quizService = quizService;
            this.quizResultService = quizResultService;
        }

        [TypeFilter(typeof(ChangeDefaoultLayoutActionFilterAttribute))]
        public IActionResult DetailsInput()
        {
            return this.View();
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> DetailsInput(InputQuizViewModel model)
        {
            var quizWithSamePasswordId = await this.quizService.GetIdByPassword(model.Password);
            if (quizWithSamePasswordId != null)
            {
                return this.View(model);
            }

            var userId = this.userManager.GetUserId(this.User);
            model.CreatorId = userId;
            model.PasswordIsValid = true;
            var quizId = await this.quizService.AddNewQuizAsync(model.Name, model.Description, model.ActivationDate, model.ActiveFrom, model.ActiveTo, model.Timer, model.CreatorId, model.Password);
            this.HttpContext.Session.SetString(Constants.QuizSeesionId, quizId);
            return this.RedirectToAction("QuestionInput", "Questions");
        }

        [HttpGet]
        [TypeFilter(typeof(ChangeDefaoultLayoutActionFilterAttribute))]
        public async Task<IActionResult> Display(string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                id = this.HttpContext.Session.GetString(Constants.QuizSeesionId);
            }

            this.HttpContext.Session.SetString(Constants.QuizSeesionId, id);

            var quizModel = await this.quizService.GetQuizByIdAsync<InputQuizViewModel>(id);

            return this.View(quizModel);
        }

        [HttpGet]
        public IActionResult Submit(QuizResultViewModel model)
        {
            return this.View(model);
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

        [HttpGet]
        public async Task<IActionResult> PDFExport(string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                id = this.HttpContext.Session.GetString(Constants.QuizSeesionId);
            }

            var quizModel = await this.quizService.GetQuizByIdAsync<InputQuizViewModel>(id);

            return new ViewAsPdf("PDFExport", quizModel)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            };
        }

        public async Task<IActionResult> DeleteQuiz(string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                id = this.HttpContext.Session.GetString(Constants.QuizSeesionId);
                await this.quizService.DeleteByIdAsync(id);
                return this.RedirectToAction("Index", "Home");
            }

            await this.quizService.DeleteByIdAsync(id);
            return this.RedirectToAction("AllQuizzesCreatedByUser", "Quizzes", new { area = "Administration" });
        }

        [HttpGet]
        [TypeFilter(typeof(ChangeDefaoultLayoutActionFilterAttribute))]
        public async Task<IActionResult> EditDetailsInput(string id)
        {
            var editModel = await this.quizService.GetQuizByIdAsync<EditDetailsViewModel>(id);
            editModel.PasswordIsValid = true;

            return this.View(editModel);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        [TypeFilter(typeof(ChangeDefaoultLayoutActionFilterAttribute))]
        public async Task<IActionResult> EditDetailsInput(EditDetailsViewModel model)
        {
            var quizWithSamePasswordId = await this.quizService.GetIdByPassword(model.Password);
            if (quizWithSamePasswordId != null && quizWithSamePasswordId != model.Id)
            {
                model.PasswordIsValid = false;
                return this.View(model);
            }

            await this.quizService.UpdateAsync(model.Id, model.Name, model.Description, model.ActivationDate, model.ActiveFrom, model.ActiveTo, model.Timer, model.Password);

            return this.RedirectToAction("Display", new { id = model.Id });
        }
    }
}
