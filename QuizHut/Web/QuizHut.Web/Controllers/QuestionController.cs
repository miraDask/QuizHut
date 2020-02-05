namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Cache;
    using QuizHut.Services.Question;
    using QuizHut.Web.ViewModels.Answer;
    using QuizHut.Web.ViewModels.Question;

    public class QuestionController : Controller
    {
        private readonly IQuestionService questionService;
        private readonly ICacheService cacheService;

        public QuestionController(IQuestionService questionService, ICacheService cacheService)
        {
            this.questionService = questionService;
            this.cacheService = cacheService;
        }

        [HttpGet]
        public IActionResult QuestionInput(QuestionViewModel questionViewModel)
        {
            return this.View(questionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewQuestion(QuestionViewModel questionViewModel)
        {
            var questionId = await this.questionService.AddNewQuestionAsync(questionViewModel);
            var answerModel = new AnswerViewModel() { QuestionId = questionId };
            return this.RedirectToAction("AnswerInput", "Answer", answerModel);
        }

        [HttpPost]
        public async Task<JsonResult> RemoveQuestion(string id)
        {
            await this.cacheService.DeleteQuestionAsync(id);
            return this.Json("Ok");
        }



        // POST: Question/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Question/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Question/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Question/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Question/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}