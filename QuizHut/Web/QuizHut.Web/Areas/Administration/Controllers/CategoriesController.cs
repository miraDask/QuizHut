namespace QuizHut.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class CategoriesController : AdministrationController
    {
        public IActionResult AllCategoriesCreatedByUser()
        {
            return this.View();
        }
    }
}