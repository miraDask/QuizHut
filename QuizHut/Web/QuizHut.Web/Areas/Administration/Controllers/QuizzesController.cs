namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Data.Models;
    using QuizHut.Services.Quizzes;
    using QuizHut.Web.Common;
    using QuizHut.Web.Filters;
    using QuizHut.Web.ViewModels.Quizzes;
    using Rotativa.AspNetCore;

    public class QuizzesController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IQuizzesService quizService;

        public QuizzesController(UserManager<ApplicationUser> userManager, IQuizzesService quizService)
        {
            this.userManager = userManager;
            this.quizService = quizService;
        }

        public IActionResult DetailsInput()
        {
            return this.View();
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> DetailsInput(InputQuizViewModel model)
        {
            var quizWithSamePasswordId = await this.quizService.GetQuizIdByPasswordAsync(model.Password);
            if (quizWithSamePasswordId != null)
            {
                return this.View(model);
            }

            var userId = this.userManager.GetUserId(this.User);
            model.CreatorId = userId;
            model.PasswordIsValid = true;
            var quizId = await this.quizService.AddNewQuizAsync(model.Name, model.Description, model.Timer, model.CreatorId, model.Password);
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

            this.HttpContext.Session.SetString(Constants.QuizSeesionId, id);

            var quizModel = await this.quizService.GetQuizByIdAsync<InputQuizViewModel>(id);

            return this.View(quizModel);
        }

        public async Task<IActionResult> AllQuizzesCreatedByTeacher()
        {
            var userId = this.userManager.GetUserId(this.User);
            var quizzes = await this.quizService.GetAllByCreatorIdAsync<QuizListViewModel>(userId, false);
            var model = new QuizzesAllListingViewModel() { Quizzes = quizzes };
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public IActionResult AllQuizzesCreatedByTeacher(QuizzesAllListingViewModel model)
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
                return this.RedirectToAction("Index", "Home", new
                {
                    password = model.Password,
                    area = GlobalConstants.Administration,
                    errorText = string.Format(GlobalConstants.ErrorMessages.QuizNotFound, model.Password),
                });
            }

            return this.RedirectToAction("Start", "Quizzes", new { area = string.Empty, password = model.Password });
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
            return this.RedirectToAction("AllQuizzesCreatedByTeacher", "Quizzes", new { area = "Administration" });
        }

        [HttpGet]
        public async Task<IActionResult> EditDetailsInput(string id)
        {
            var editModel = await this.quizService.GetQuizByIdAsync<EditDetailsViewModel>(id);
            editModel.PasswordIsValid = true;

            return this.View(editModel);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> EditDetailsInput(EditDetailsViewModel model)
        {
            var quizWithSamePasswordId = await this.quizService.GetQuizIdByPasswordAsync(model.Password);
            if (quizWithSamePasswordId != null && quizWithSamePasswordId != model.Id)
            {
                model.PasswordIsValid = false;
                return this.View(model);
            }

            await this.quizService.UpdateAsync(model.Id, model.Name, model.Description, model.Timer, model.Password);

            return this.RedirectToAction("Display", new { id = model.Id });
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
    }
}
