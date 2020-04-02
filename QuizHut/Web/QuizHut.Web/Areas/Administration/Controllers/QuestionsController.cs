namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
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
        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
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
        public async Task<IActionResult> AddNewQuestion(QuestionInputModel model)
        {
            var quizId = this.HttpContext.Session.GetString(Constants.QuizSeesionId);
            var questionId = await this.questionService.CreateQuestionAsync(quizId, model.Text);
            this.HttpContext.Session.SetString(Constants.CurrentQuestionId, questionId);
            return this.RedirectToAction("AnswerInput", "Answers");
        }

        [HttpGet]
        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public async Task<IActionResult> EditQuestionInput(string id)
        {
            var model = await this.questionService.GetByIdAsync<QuestionInputModel>(id);
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> Edit(QuestionInputModel model)
        {
            await this.questionService.Update(model.Id, model.Text);
            var page = this.HttpContext.Session.GetInt32(GlobalConstants.PageToReturnTo);
            return this.RedirectToAction("Display", "Quizzes", new { page });
        }

        public async Task<IActionResult> Delete(string id)
        {
            await this.questionService.DeleteQuestionByIdAsync(id);

            return this.RedirectToAction("Display", "Quizzes");
        }
    }
}
