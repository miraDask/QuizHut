namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Events;
    using QuizHut.Services.Groups;
    using QuizHut.Services.Users;
    using QuizHut.Web.Filters;
    using QuizHut.Web.ViewModels.Events;
    using QuizHut.Web.ViewModels.Groups;

    public class EventsController : AdministrationController
    {
        private readonly IEventsService service;
        private readonly IUsersService userService;
        private readonly IGroupsService groupsService;
        private readonly UserManager<ApplicationUser> userManager;

        public EventsController(
            IEventsService service,
            IUsersService userService,
            IGroupsService groupsService,
            UserManager<ApplicationUser> userManager)
        {
            this.service = service;
            this.userService = userService;
            this.groupsService = groupsService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> AllEventsCreatedByTeacher()
        {
            var userId = this.userManager.GetUserId(this.User);
            var events = await this.service.GetAllByCreatorIdAsync<EventListViewModel>(userId);
            var model = new EventsListAllViewModel { Events = events };
            return this.View(model);
        }

        public IActionResult EventInput()
        {
            return this.View();
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> EventInput(CreateEventInputViewModel model)
        {
            var userId = this.userManager.GetUserId(this.User);
            var eventId = await this.service.AddNewEventAsync(model.Name, model.ActivationDate, model.ActiveFrom, model.ActiveTo, userId);

            return this.RedirectToAction("AssignGroupsToEvent", new { id = eventId });
        }

        public async Task<IActionResult> AssignGroupsToEvent(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var groups = await this.groupsService.GetAllByCreatorIdAsync<GroupAssignViewModel>(userId);
            var model = await this.service.GetEventModelAsync(id, userId, groups);

            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AssignGroupsToEvent(EventWithGroupsViewModel model)
        {
            var groupIds = model.Groups.Where(x => x.IsAssigned).Select(x => x.Id).ToList();
            await this.service.AssignGroupsToEventAsync(model.Id, groupIds);
            return this.RedirectToAction("EventDetails", new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> EventDetails(string id)
        {
            var model = await this.service.GetEventDetailsModelAsync(id);
            return this.View(model);
        }
    }
}
