namespace QuizHut.Web.Controllers
{
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Answer;
    using QuizHut.Services.Cache;
    using QuizHut.Web.ViewModels.Answer;

    public class AnswerController : Controller
    {
        private readonly IAnswerService answerService;
        private readonly ICacheService cacheService;

        public AnswerController(IAnswerService answerService, ICacheService cacheService)
        {
            this.answerService = answerService;
            this.cacheService = cacheService;
        }

        public IActionResult AnswerInput()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult AddNewAnswerAjaxCall(string questionId, string answerId)
        {
            var answer = new AnswerViewModel() { QuestionId = questionId, Id = answerId};
            var quizViewModel = this.cacheService.GetQuizModelFromCache();
            quizViewModel.Questions.Where(x => x.Id == questionId).FirstOrDefault().Answers.Add(answer);
            this.cacheService.SaveQuizModelToCache(quizViewModel);
            return this.PartialView("_AnswerDetailsPartial", answer);
        }

        [HttpPost]
        public IActionResult AddNewAnswer(AnswerViewModel answerViewModel)
        {

            var quizViewModel = this.cacheService.GetQuizModelFromCache();
            quizViewModel.Questions.Last().Answers.Add(answerViewModel);
            this.cacheService.SaveQuizModelToCache(quizViewModel);

            return this.RedirectToAction("AnswerInput", "Answer");
        }

        [HttpPost]
        public JsonResult RemoveAnswer(string id)
        {
            this.cacheService.DeleteAnswer(id);
            return this.Json("Ok");
        }
    }
}