namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
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

            return this.RedirectToAction("Display", "Quizzes");
        }

        [HttpGet]
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

            return this.RedirectToAction("Display", "Quizzes");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            await this.answerService.DeleteAsync(id);

            return this.RedirectToAction("Display", "Quizzes");
        }
    }
}
