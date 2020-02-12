namespace QuizHut.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Cache;
    using QuizHut.Services.Question;
    using QuizHut.Web.Controllers.Common;
    using QuizHut.Web.ViewModels.Questions;
    using ReflectionIT.Mvc.Paging;

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

        [HttpGet]
        public async Task<IActionResult> Index(int page = Constants.DefaultPage)
        {
            this.HttpContext.Session.SetInt32(Constants.CurrentQuestionNumber, page);

            var attemptedQuiz = this.HttpContext.Session.GetString(Constants.AttemptedQuizId);

            var query = this.questionService.GetAllQuestionsQuizById(attemptedQuiz);
            var model = await PagingList.CreateAsync(query, Constants.DefaultPage, page);
            return this.View(model);
        }

        [HttpPost]
        public IActionResult Index(IFormCollection collection)
        {
            var assumtions = collection.Where(x => x.Key.Contains(Constants.IsRightAnswerAssumption));
            var rightAnswers = collection.Where(x => x.Key.Contains(Constants.IsRightAnswer) && !x.Key.Contains(Constants.IsRightAnswerAssumption));
            var result = this.questionService.CalculateQuestionResult(assumtions, rightAnswers);
            var quizResult = this.HttpContext.Session.GetInt32(Constants.QuizResult);

            if (quizResult == null)
            {
                this.HttpContext.Session.SetInt32(Constants.QuizResult, result);
            }
            else
            {
                this.HttpContext.Session.SetInt32(Constants.QuizResult, (int)quizResult + result);
            }

            var questionsCount = this.HttpContext.Session.GetInt32(Constants.QuestionsCount);
            var currentQuestionNumber = this.HttpContext.Session.GetInt32(Constants.CurrentQuestionNumber);

            if (currentQuestionNumber == questionsCount)
            {
                return this.RedirectToAction("DisplayResult", "Quizzes");
            }
            else
            {
                return this.RedirectToAction("Index", new { page = ++currentQuestionNumber });
            }
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