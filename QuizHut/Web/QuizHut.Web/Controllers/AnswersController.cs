namespace QuizHut.Web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Answer;
    using QuizHut.Services.Cache;
    using QuizHut.Web.Controllers.Common;
    using QuizHut.Web.ViewModels.Answers;
    using System.Threading.Tasks;

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

        [HttpPost]
        public IActionResult AnswerInput(string id)
        {
            var answerToAdd = true;

            if (id == null)
            {
                answerToAdd = false;
            }

            var model = new AnswerViewModel() { QuestionId = id, AnswerToAdd = answerToAdd };
            return this.View(model);
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
            if (!model.AnswerToAdd)
            {
                var currentQuestionId = this.HttpContext.Session.GetString(Constants.CurrentQuestionId);
                await this.answerService.AddNewAnswerAsync(model.Text, model.IsRightAnswer, currentQuestionId);
                return this.RedirectToAction("AnswerInput");
            }
            else
            {
                await this.answerService.AddNewAnswerAsync(model.Text, model.IsRightAnswer, model.QuestionId);
                return this.RedirectToAction("Display", "Quizzes");
            }
        }

        [HttpGet]
        public IActionResult EditAnswerInput(EditAnswerViewModel model)
        {
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAnswerInput(AnswerViewModel model)
        {
            var answerId = await this.answerService.GetAnswerId(model.Id, model.Text);

            var editModel = new EditAnswerViewModel()
            {
                Id = answerId,
                Text = model.Text,
                IsRightAnswer = model.IsRightAnswer,
            };

            return this.View(editModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(EditAnswerViewModel model)
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