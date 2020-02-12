namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Answer;
    using QuizHut.Services.Cache;
    using QuizHut.Web.Controllers.Common;
    using QuizHut.Web.ViewModels.Answers;
    using QuizHut.Web.ViewModels.Questions;

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


        [HttpPost]
        public async Task<IActionResult> EditAnswerInput(string id, string questionId, string text, string isRightAnswer)
        {
            //var answerId = await this.answerService.GetAnswerId(model.Id, model.Text);
            //model.Id = answerId;
            
            return this.View(/*model*/);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AnswerViewModel model)
        {
            await this.answerService.UpdateAsync(model.Id, model.Text, model.IsRightAnswer);
            return this.RedirectToAction("Display", "Quizzes");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(AnswerViewModel model)
        {
            var answerId = await this.answerService.GetAnswerId(model.Id, model.Text);
            await this.answerService.Delete(answerId);

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