namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Answer;
    using QuizHut.Services.Cache;
    using QuizHut.Web.Controllers.Common;
    using QuizHut.Web.ViewModels.Answer;

    public class AnswerController : Controller
    {
        private readonly IAnswerService answerService;

        public AnswerController(IAnswerService answerService, ICacheService cacheService)
        {
            this.answerService = answerService;

        }

        [HttpGet]
        public IActionResult AnswerInput()
        {
            return this.View();
        }

        //[HttpPost]
        //public async Task<IActionResult> AddNewAnswerAjaxCall(string questionId, string answerId)
        //{
        //    var answer = new AnswerViewModel() { QuestionId = questionId, Id = answerId };
        //    var quizViewModel = await this.cacheService.GetQuizModelFromCacheAsync();
        //    quizViewModel.Questions.Where(x => x.Id == questionId).FirstOrDefault().Answers.Add(answer);
        //    await this.cacheService.SaveQuizModelToCacheAsync(quizViewModel);
        //    return this.PartialView("_AnswerDetailsPartial", answer);
        //}

        [HttpPost]
        public async Task<IActionResult> AddNewAnswer(AnswerViewModel model)
        {
            var currentQuestionId = this.HttpContext.Session.GetString(Constants.CurrentQuestionId);
            await this.answerService.AddNewAnswerAsync(model.Text, model.IsRightAnswer, currentQuestionId);

            return this.RedirectToAction("AnswerInput");
        }

        //[HttpPost]
        //public async Task<JsonResult> RemoveAnswer(string id)
        //{
        //    await this.cacheService.DeleteAnswerAsync(id);
        //    return this.Json("Ok");
        //}
    }
}