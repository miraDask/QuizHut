namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Events;
    using QuizHut.Web.ViewModels.Common;
    using QuizHut.Web.ViewModels.Events;

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
            var model = new EventsListAllViewModel
            {
                Events = events.Where(x => x.Status[ModelCostants.Status] != ModelCostants.StatusPending),
            };

            return this.View(model);
        }

        public async Task<IActionResult> EventResultsDetails(string id)
        {
            var eventModel = await this.eventService.GetEventModelByIdAsync<EventWithGroupAndQuizNamesViewModel>(id);

            return this.View(eventModel);
        }
    }
}
