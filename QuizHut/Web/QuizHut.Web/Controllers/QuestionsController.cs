namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Cache;
    using QuizHut.Services.Question;
    using QuizHut.Web.Controllers.Common;
    using QuizHut.Web.ViewModels.Questions;

    public class QuestionsController : Controller
    {
        private readonly IQuestionService questionService;

        public QuestionsController(IQuestionService questionService, ICacheService cacheService)
        {
            this.questionService = questionService;
        }

        [HttpGet]
        public IActionResult QuestionInput()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewQuestion(QuestionViewModel model)
        {
            var quizId = this.HttpContext.Session.GetString(Constants.QuizSeesionId);
            var questionId = await this.questionService.AddNewQuestionAsync(quizId, model.Text);
            this.HttpContext.Session.SetString(Constants.CurrentQuestionId, questionId);
            return this.RedirectToAction("AnswerInput", "Answers");
        }

        [HttpPost]
        public IActionResult EditQuestionInput(QuestionViewModel model)
        {
            return this.View(model);
        }

        public async Task<IActionResult> Edit(QuestionViewModel model)
        {
            await this.questionService.Update(model.Id, model.Text);

            return this.RedirectToAction("Display", "Quizzes");
        }

        public async Task<IActionResult> Delete(QuestionViewModel model)
        {
            await this.questionService.DeleteQuestionByIdAsync(model.Id);

            return this.RedirectToAction("Display", "Quizzes");
        }
    }
}
