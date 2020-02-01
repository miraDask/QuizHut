namespace QuizHut.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Question;
    using QuizHut.Web.ViewModels.Question;

    public class QuestionController : Controller
    {
        private readonly IQuestionService questionService;

        public QuestionController(IQuestionService questionService)
        {
            this.questionService = questionService;
        }

        [HttpGet]
        public async Task<ActionResult> QuestionInput(int q)
        {
            if (q == 0)
            {
                var previousQuestioId = (int)this.TempData["QuestionId"];
                q = await this.questionService.GetQuizIdByQuestionIdAsync(previousQuestioId);
            }

            var questionViewModel = new QuestionViewModel();
            questionViewModel.QuizId = q;
            return this.View(questionViewModel);
        }

        // GET: Question/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // GET: Question/Create
        [HttpPost]
        public async Task<ActionResult> Create(QuestionViewModel questionViewModel)
        {
            questionViewModel.QuizId = (int)this.TempData["QuizId"];
            var questionId = await this.questionService.AddNewQuestionAsync(questionViewModel);

            return this.RedirectToAction("AnswerInput", "Answer", new { q = questionId });
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