namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Cache;
    using QuizHut.Services.Question;
    using QuizHut.Web.ViewModels.Question;
    using Microsoft.AspNetCore.Http;
    using QuizHut.Web.Controllers.Common;
    using ReflectionIT.Mvc.Paging;

    public class QuestionController : Controller
    {
        private readonly IQuestionService questionService;

        public QuestionController(IQuestionService questionService, ICacheService cacheService)
        {
            this.questionService = questionService;
        }

        [HttpGet]
        public IActionResult QuestionInput()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewQuestion(QuestionViewModel questionViewModel)
        {
            questionViewModel.QuizId = this.HttpContext.Session.GetString(Constants.QuizSeesionId);
            var questionId = await this.questionService.AddNewQuestionAsync(questionViewModel);
            this.HttpContext.Session.SetString(Constants.CurrentQuestionId, questionId);
            return this.RedirectToAction("AnswerInput", "Answer");
        }

        [HttpGet]
        public async Task<IActionResult> Attempt(int page = Constants.DefaultPage)
        {
            var attemptedQuiz = this.HttpContext.Session.GetString(Constants.AttemptedQuizId);
            var query = this.questionService.GetAllQuestionsQuizById(attemptedQuiz);
            var model = await PagingList.CreateAsync(query, Constants.DefaultPage, page);
            return this.View(model);
        }

        //[HttpPost]
        //public async Task<JsonResult> RemoveQuestion(string id)
        //{
        //    await this.cacheService.DeleteQuestionAsync(id);
        //    return this.Json("Ok");
        //}



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