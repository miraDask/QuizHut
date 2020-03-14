namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Events;
    using QuizHut.Services.Groups;
    using QuizHut.Services.Quizzes;
    using QuizHut.Web.Filters;
    using QuizHut.Web.ViewModels.Common;
    using QuizHut.Web.ViewModels.Events;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Quizzes;

    public class EventsController : AdministrationController
    {
        private readonly IEventsService service;
        private readonly IQuizzesService quizService;
        private readonly IGroupsService groupsService;
        private readonly UserManager<ApplicationUser> userManager;

        public EventsController(
            IEventsService service,
            IQuizzesService quizService,
            IGroupsService groupsService,
            UserManager<ApplicationUser> userManager)
        {
            this.service = service;
            this.quizService = quizService;
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
            var timeErrorMessage = this.service.GetTimeErrorMessage(model.ActiveFrom, model.ActiveTo, model.ActivationDate);
            if (timeErrorMessage != null)
            {
                model.Error = timeErrorMessage;
                return this.View(model);
            }

            var userId = this.userManager.GetUserId(this.User);
            var eventId = await this.service.AddNewEventAsync(model.Name, model.ActivationDate, model.ActiveFrom, model.ActiveTo, userId);
            return this.RedirectToAction("AssignGroupsToEvent", new { id = eventId });
        }

        public async Task<IActionResult> AssignGroupsToEvent(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var groups = await this.groupsService.GetAllByCreatorIdAsync<GroupAssignViewModel>(userId);
            var model = await this.service.GetEventModelByIdAsync<EventWithGroupsViewModel>(id);
            model.Groups = groups;
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AssignGroupsToEvent(EventWithGroupsViewModel model)
        {
            var groupIds = model.Groups.Where(x => x.IsAssigned).Select(x => x.Id).ToList();

            if (groupIds.Count == 0)
            {
                model.Error = true;
                return this.View(model);
            }

            await this.groupsService.AssignEventsToGroupAsync(groupIds[0], new List<string>() { model.Id});
            return this.RedirectToAction("AssignQuizToEvent", new { id = model.Id });
        }

        public async Task<IActionResult> AddGroupsToEvent(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var groups = await this.groupsService.GetAllByCreatorIdAsync<GroupAssignViewModel>(userId, id);
            var model = await this.service.GetEventModelByIdAsync<EventWithGroupsViewModel>(id);
            model.Groups = groups;
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AddGroupsToEvent(EventWithGroupsViewModel model)
        {
            var groupIds = model.Groups.Where(x => x.IsAssigned).Select(x => x.Id).ToList();

            if (groupIds.Count == 0)
            {
                model.Error = true;
                return this.View(model);
            }

            await this.service.AssignGroupsToEventAsync(groupIds, model.Id);
            return this.RedirectToAction("EventDetails", new { id = model.Id });
        }

        public async Task<IActionResult> AssignQuizToEvent(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var quizzes = await this.quizService.GetAllByCreatorIdAsync<QuizAssignViewModel>(userId, true);
            var model = await this.service.GetEventModelByIdAsync<EventWithQuizzesViewModel>(id);
            model.Quizzes = quizzes;
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AssignQuizToEvent(EventWithQuizzesViewModel model)
        {
            var quizzes = model.Quizzes.Where(x => x.IsAssigned).ToList();

            if (quizzes.Count() != 1)
            {
                model.Error = true;
                return this.View(model);
            }

            await this.service.AssigQuizToEventAsync(model.Id, quizzes[0].Id);
            return this.RedirectToAction("EventDetails", new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> EventDetails(string id)
        {
            var groups = await this.groupsService.GetGroupModelsAllByEventIdAsync<GroupAssignViewModel>(id);
            var model = await this.service.GetEventModelByIdAsync<EventDetailsViewModel>(id);
            model.Groups = groups;

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            await this.service.DeleteAsync(id);
            return this.RedirectToAction("AllEventsCreatedByTeacher");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuizFromEvent(string eventId, string quizId)
        {
            await this.service.DeleteQuizFromEventAsync(eventId, quizId);
            return this.RedirectToAction("EventDetails", new { id = eventId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGroupFromEvent(string groupId, string eventId)
        {
            await this.groupsService.DeleteEventFromGroupAsync(groupId, eventId);
            return this.RedirectToAction("EventDetails", new { id = eventId });
        }

        public async Task<IActionResult> ActiveEventsAll()
        {
            var userId = this.userManager.GetUserId(this.User);
            var events = await this.service.GetAllByCreatorIdAsync<EventListViewModel>(userId);
            var model = new EventsListAllViewModel
            {
                Events = events.Where(x => x.Status[ModelCostants.Name] == ModelCostants.StatusActive),
            };

            return this.View(model);
        }

        public async Task<IActionResult> EndedEventsAll()
        {
            var userId = this.userManager.GetUserId(this.User);
            var events = await this.service.GetAllByCreatorIdAsync<EventListViewModel>(userId);
            var model = new EventsListAllViewModel
            {
                Events = events.Where(x => x.Status[ModelCostants.Name] == ModelCostants.StatusEnded),
            };

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditEventDetails(string id)
        {
            var editModel = await this.service.GetEventModelByIdAsync<EditEventDetailsInputViewModel>(id);

            return this.View(editModel);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> EditEventDetails(EditEventDetailsInputViewModel model)
        {
            var timeErrorMessage = this.service.GetTimeErrorMessage(model.ActiveFrom, model.ActiveTo, model.ActivationDate);
            if (timeErrorMessage != null)
            {
                model.Error = timeErrorMessage;
                return this.View(model);
            }

            await this.service.UpdateAsync(model.Id, model.Name, model.ActivationDate, model.ActiveFrom, model.ActiveTo);

            return this.RedirectToAction("EventDetails", new { id = model.Id });
        }
    }
}
