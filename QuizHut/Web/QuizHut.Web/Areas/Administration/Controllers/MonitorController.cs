namespace QuizHut.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class MonitorController : AdministrationController
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}