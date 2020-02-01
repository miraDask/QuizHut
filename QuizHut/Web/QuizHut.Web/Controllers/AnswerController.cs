namespace QuizHut.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Web.ViewModels.Answer;

    public class AnswerController : Controller
    {
        public IActionResult AnswerInput(int q)
        {
            var answer = new AnswerViewModel { QuestionId = q };
            return this.View(answer);
        }

        [HttpPost]
        public IActionResult Create(AnswerViewModel answerViewModel)
        {
            answerViewModel.QuestionId = (int)this.TempData["QuestionId"];
            return this.RedirectToAction("AnswerInput", "Answer", new { q = answerViewModel.QuestionId });
        }
    }
}