namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Answer;
    using QuizHut.Web.Controllers.Common;
    using QuizHut.Web.ViewModels.Answers;

    public class AnswersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAnswerService answerService;

        public AnswersController(
            UserManager<ApplicationUser> userManager,
            IAnswerService answerService)
        {
            this.userManager = userManager;
            this.answerService = answerService;
        }

        [HttpGet]
        public IActionResult AnswerInput()
        {
            if (this.userManager.GetUserId(this.User) != null)
            {
                this.ViewData["Layout"] = Constants.AdminLayout;
            }

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewAnswer(AnswerViewModel model)
        {
            var currentQuestionId = this.HttpContext.Session.GetString(Constants.CurrentQuestionId);
            await this.answerService.AddNewAnswerAsync(model.Text, model.IsRightAnswer, currentQuestionId);

            return this.RedirectToAction("AnswerInput");
        }

        [HttpPost]
        public IActionResult ApendAnswerInput(string id)
        {
            if (this.userManager.GetUserId(this.User) != null)
            {
                this.ViewData["Layout"] = Constants.AdminLayout;
            }

            var model = new AnswerViewModel() { QuestionId = id };
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AppendNewAnswer(AnswerViewModel model)
        {
            await this.answerService.AddNewAnswerAsync(model.Text, model.IsRightAnswer, model.QuestionId);
            if (this.userManager.GetUserId(this.User) != null)
            {
                this.ViewData["Layout"] = Constants.AdminLayout;
            }

            return this.RedirectToAction("Display", "Quizzes");
        }


        [HttpGet]
        public async Task<IActionResult> EditAnswerInput(string id)
        {
            if (this.userManager.GetUserId(this.User) != null)
            {
                this.ViewData["Layout"] = Constants.AdminLayout;
            }

            var model = await this.answerService.GetAnswerModelAsync(id);

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AnswerViewModel model)
        {
            await this.answerService.UpdateAsync(model.Id, model.Text, model.IsRightAnswer);
            return this.RedirectToAction("Display", "Quizzes");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            await this.answerService.Delete(id);

            return this.RedirectToAction("Display", "Quizzes");
        }
    }
}