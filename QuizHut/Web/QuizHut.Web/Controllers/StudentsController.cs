namespace QuizHut.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Services.Events;
    using QuizHut.Services.Results;
    using QuizHut.Web.ViewModels.Events;
    using QuizHut.Web.ViewModels.Quizzes;
    using QuizHut.Web.ViewModels.Results;

    [Authorize]
    public class StudentsController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IResultsService resultService;
        private readonly IEventsService eventsService;

        public StudentsController(
            UserManager<ApplicationUser> userManager,
            IResultsService resultService,
            IEventsService eventsService)
        {
            this.userManager = userManager;
            this.resultService = resultService;
            this.eventsService = eventsService;
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

        public async Task<IActionResult> StudentActiveEventsAll()
        {
            var studentId = this.userManager.GetUserId(this.User);
            var activeEvents = await this.eventsService
                .GetAllFiteredByStatusAsync<StudentActiveEventViewModel>(Status.Active, null, studentId);

            return this.View(activeEvents);
        }

        public async Task<IActionResult> StudentEndedEventsAll()
        {
            var studentId = this.userManager.GetUserId(this.User);
            var endedEvents = await this.eventsService
                .GetAllFiteredByStatusAsync<StudentEndedEventViewModel>(Status.Ended, null, studentId);
            var scores = await this.resultService.GetAllByStudentIdAsync<ScoreViewModel>(studentId);
            foreach (var endenEvent in endedEvents)
            {
                endenEvent.Score = scores.FirstOrDefault(x => x.EventId == endenEvent.Id);
            }

            return this.View(endedEvents);
        }

        public async Task<IActionResult> StudentPendingEventsAll()
        {
            var studentId = this.userManager.GetUserId(this.User);
            var activeEvents = await this.eventsService
                .GetAllFiteredByStatusAsync<StudentPendingEventViewModel>(Status.Pending, null, studentId);

            return this.View(activeEvents);
        }
    }
}
