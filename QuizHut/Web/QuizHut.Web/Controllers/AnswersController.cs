namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Answer;
    using QuizHut.Services.Cache;
    using QuizHut.Web.Controllers.Common;
    using QuizHut.Web.ViewModels.Answers;

    public class AnswersController : Controller
    {
        private readonly IAnswerService answerService;

        public AnswersController(IAnswerService answerService, ICacheService cacheService)
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

        [HttpPost]
        public IActionResult ApendAnswerInput(string id)
        {
            var model = new AnswerViewModel() { QuestionId = id };
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AppendNewAnswer(AnswerViewModel model)
        {
            await this.answerService.AddNewAnswerAsync(model.Text, model.IsRightAnswer, model.QuestionId);
            return this.RedirectToAction("Display", "Quizzes");
        }


        [HttpGet]
        public async Task<IActionResult> EditAnswerInput(string id)
        {
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

        //[HttpPost]
        //public async Task<JsonResult> RemoveAnswer(string id)
        //{
        //    await this.cacheService.DeleteAnswerAsync(id);
        //    return this.Json("Ok");
        //}
    }
}