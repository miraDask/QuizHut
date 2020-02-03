namespace QuizHut.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Cache;
    using QuizHut.Services.Question;
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
        public IActionResult QuestionInput()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult AddNewQuestion(QuestionViewModel questionViewModel)
        {
            var quizViewModel = this.cacheService.GetQuizModelFromCache();
            quizViewModel.Questions.Add(questionViewModel);
            this.cacheService.SaveQuizModelToCache(quizViewModel);

            return this.RedirectToAction("AnswerInput", "Answer");
        }

        [HttpPost]
        public JsonResult RemoveQuestion(string id)
        {
            this.cacheService.DeleteQuestion(id);
            var model = this.cacheService.GetQuizModelFromCache();
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