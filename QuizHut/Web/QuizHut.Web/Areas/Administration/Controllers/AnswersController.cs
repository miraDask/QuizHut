namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Services.Answers;
    using QuizHut.Web.Common;
    using QuizHut.Web.Infrastructure.Filters;
    using QuizHut.Web.ViewModels.Answers;

    public class AnswersController : AdministrationController
    {
        private readonly IAnswersService answerService;

        public AnswersController(IAnswersService answerService)
        {
            this.answerService = answerService;
        }

        [HttpGet]
        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public IActionResult AnswerInput()
        {
            return this.View();
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AddNewAnswer(AnswerViewModel model)
        {
            var currentQuestionId = this.HttpContext.Session.GetString(Constants.CurrentQuestionId);
            await this.answerService.CreateAnswerAsync(model.Text, model.IsRightAnswer, currentQuestionId);

            return this.RedirectToAction("AnswerInput");
        }

        [HttpGet]
        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public IActionResult ApendAnswerInput(string id)
        {
            var model = new AnswerViewModel() { QuestionId = id };

            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AppendNewAnswer(AnswerViewModel model)
        {
            await this.answerService.CreateAnswerAsync(model.Text, model.IsRightAnswer, model.QuestionId);
            var page = this.HttpContext.Session.GetInt32(GlobalConstants.PageToReturnTo);

            return this.RedirectToAction("Display", "Quizzes", new { page });
        }

        [HttpGet]
        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public async Task<IActionResult> EditAnswerInput(string id)
        {
            var model = await this.answerService.GetByIdAsync<AnswerViewModel>(id);

            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> Update(AnswerViewModel model)
        {
            await this.answerService.UpdateAsync(model.Id, model.Text, model.IsRightAnswer);
            var page = this.HttpContext.Session.GetInt32(GlobalConstants.PageToReturnTo);
            return this.RedirectToAction("Display", "Quizzes", new { page });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            await this.answerService.DeleteAsync(id);
            var page = this.HttpContext.Session.GetInt32(GlobalConstants.PageToReturnTo);
            return this.RedirectToAction("Display", "Quizzes", new { page });
        }
    }
}
