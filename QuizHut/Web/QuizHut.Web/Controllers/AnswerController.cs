namespace QuizHut.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Answer;
    using QuizHut.Services.Cache;
    using QuizHut.Web.ViewModels.Answer;

    public class AnswerController : Controller
    {
        private readonly IAnswerService answerService;

        public AnswerController(IAnswerService answerService, ICacheService cacheService)
        {
            this.answerService = answerService;

        }

        [HttpGet]
        public IActionResult AnswerInput(AnswerViewModel answerViewModel)
        {
            return this.View(answerViewModel);
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
        public async Task<IActionResult> AddNewAnswer(AnswerViewModel answerViewModel)
        {
            var currentQuestionId = await this.answerService.AddNewAnswerAsync(answerViewModel);
            var answerModel = new AnswerViewModel() { QuestionId = currentQuestionId };
            return this.RedirectToAction("AnswerInput", answerModel);
        }

        //[HttpPost]
        //public async Task<JsonResult> RemoveAnswer(string id)
        //{
        //    await this.cacheService.DeleteAnswerAsync(id);
        //    return this.Json("Ok");
        //}
    }
}