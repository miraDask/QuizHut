namespace QuizHut.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Data.Models;
    using QuizHut.Services.Results;
    using QuizHut.Web.Filters;
    using QuizHut.Web.ViewModels.Quizzes;
    using QuizHut.Web.ViewModels.Results;

    [ChangeDefaoultLayoutActionFilterAttribute]
    public class StudentsController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IResultsService resultService;

        public StudentsController(
            UserManager<ApplicationUser> userManager,
            IResultsService resultService)
        {
            this.userManager = userManager;
            this.resultService = resultService;
        }

        public IActionResult Index(string password, string errorText)
        {
            var model = new PasswordInputViewModel();
            if (errorText != null)
            {
                model.Password = password;
                model.Error = errorText;
            }

            return this.View(model);
        }

        public async Task<IActionResult> Results()
        {
            var studentId = this.userManager.GetUserId(this.User);
            var resultsModel = await this.resultService.GetAllByStudentIdAsync<StudentResultViewModel>(studentId);
            return this.View(resultsModel);
        }
    }
}