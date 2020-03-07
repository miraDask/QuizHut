namespace QuizHut.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Events;
    using QuizHut.Web.ViewModels.Events;
    using System.Threading.Tasks;

    public class ResultsController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEventsService eventService;

        public ResultsController(UserManager<ApplicationUser> userManager, IEventsService eventService)
        {
            this.userManager = userManager;
            this.eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = this.userManager.GetUserId(this.User);
            var events = await this.eventService.GetAllByCreatorIdAsync<EventListViewModel>(userId);
            var model = new EventsListAllViewModel { Events = events };
            return this.View(model);
        }

        public async Task<IActionResult> ActiveResultsAll()
        {
            return this.View();
        }

        public async Task<IActionResult> EndedResultsAll()
        {
            return this.View();
        }
    }
}