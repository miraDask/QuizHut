namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Questions;
    using QuizHut.Web.Common;
    using QuizHut.Web.Infrastructure.Filters;
    using QuizHut.Web.ViewModels.Questions;

    public class QuestionsController : AdministrationController
    {
        private readonly IQuestionsService questionService;

        public QuestionsController(IQuestionsService questionService)
        {
            this.questionService = questionService;
        }

        [HttpGet]
        public IActionResult QuestionInput(string id)
        {
            if (id != null)
            {
                this.HttpContext.Session.SetString(Constants.QuizSeesionId, id);
            }

            return this.View();
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AddNewQuestion(QuestionViewModel model)
        {
            var quizId = this.HttpContext.Session.GetString(Constants.QuizSeesionId);
            var questionId = await this.questionService.CreateQuestionAsync(quizId, model.Text);
            this.HttpContext.Session.SetString(Constants.CurrentQuestionId, questionId);
            return this.RedirectToAction("AnswerInput", "Answers");
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public IActionResult EditQuestionInput(QuestionViewModel model)
        {
            return this.View(model);
        }

        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> Edit(QuestionViewModel model)
        {
            await this.questionService.Update(model.Id, model.Text);

            return this.RedirectToAction("Display", "Quizzes");
        }

        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> Delete(QuestionViewModel model)
        {
            await this.questionService.DeleteQuestionByIdAsync(model.Id);

            return this.RedirectToAction("Display", "Quizzes");
        }
    }
}
