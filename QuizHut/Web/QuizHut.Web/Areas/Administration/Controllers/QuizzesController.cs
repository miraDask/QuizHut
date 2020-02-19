namespace QuizHut.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class QuizzesController : AdministrationController
    {
        public IActionResult AllQuizzesCreatedByUser()
        {
            return this.View();
        }
    }
}