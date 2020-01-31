namespace QuizHut.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    public class AnswerController : Controller
    {
        public IActionResult AnswerInput()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Create()
        {
            return this.View("AnswerInput");
        }
    }
}