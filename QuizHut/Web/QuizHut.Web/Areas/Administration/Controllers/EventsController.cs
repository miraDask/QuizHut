namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Services.Events;
    using QuizHut.Services.Groups;
    using QuizHut.Services.Quizzes;
    using QuizHut.Web.Infrastructure.Filters;
    using QuizHut.Web.ViewModels.Events;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Quizzes;

    public class EventsController : AdministrationController
    {
        private const int PerPageDefaultValue = 5;
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

        [ClearDashboardRequestInSessionActionFilterAttribute]
        public async Task<IActionResult> AllEventsCreatedByTeacher(int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var userId = this.userManager.GetUserId(this.User);
            var allEventsCreatedByTeacher = this.service.GetAllEventsCount(userId);
            int pagesCount = 0;

            var model = new EventsListAllViewModel()
            {
                CurrentPage = page,
                PagesCount = pagesCount,
            };

            if (allEventsCreatedByTeacher > 0)
            {
                pagesCount = (int)Math.Ceiling(allEventsCreatedByTeacher / (decimal)countPerPage);
                var events = await this.service.GetAllPerPage<EventListViewModel>(page, countPerPage, userId);
                model.Events = events;
                model.PagesCount = pagesCount;
            }

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
            var eventId = await this.service.CreateEventAsync(model.Name, model.ActivationDate, model.ActiveFrom, model.ActiveTo, userId);
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

            await this.groupsService.AssignEventsToGroupAsync(groupIds[0], new List<string>() { model.Id });
            return this.RedirectToAction("AssignQuizToEvent", new { id = model.Id });
        }

        public async Task<IActionResult> AddGroupsToEvent(string id)
        {
            var userId = this.userManager.GetUserId(this.User);

            IList<GroupAssignViewModel> groups;
            var isDashboardRequest = this.HttpContext.Session.GetString("DashboardRequest") != null;
            if (isDashboardRequest)
            {
                groups = await this.groupsService.GetAllAsync<GroupAssignViewModel>(id);
            }
            else
            {
                groups = await this.groupsService.GetAllByCreatorIdAsync<GroupAssignViewModel>(userId, id);
            }

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
            IList<QuizAssignViewModel> quizzes;
            var isDashboardRequest = this.HttpContext.Session.GetString("DashboardRequest") != null;
            if (isDashboardRequest)
            {
                quizzes = await this.quizService.GetAllAsync<QuizAssignViewModel>(true);
            }
            else
            {
                quizzes = await this.quizService.GetAllByCreatorIdAsync<QuizAssignViewModel>(userId, true);
            }

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
        public async Task<IActionResult> EventDetails(string id, string messagesAreSend)
        {
            var groups = await this.groupsService.GetGroupModelsAllByEventIdAsync<GroupAssignViewModel>(id);
            var model = await this.service.GetEventModelByIdAsync<EventDetailsViewModel>(id);
            model.Groups = groups;

            if (messagesAreSend != null)
            {
                model.ConfirmationMessage = messagesAreSend;
            }

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
            var activeEvents = await this.service.GetAllFiteredByStatusAsync<EventListViewModel>(Status.Active, userId);
            var model = new EventsListAllViewModel
            {
                Events = activeEvents,
            };

            return this.View(model);
        }

        public async Task<IActionResult> EndedEventsAll()
        {
            var userId = this.userManager.GetUserId(this.User);
            var endedEvents = await this.service.GetAllFiteredByStatusAsync<EventListViewModel>(Status.Ended, userId);
            var model = new EventsListAllViewModel
            {
                Events = endedEvents,
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

        public async Task<IActionResult> SendMessageToGroupMembers(string id)
        {
            string path = "./wwwroot/html/email.html";
            string emailHtmlContent = System.IO.File.ReadAllText(path);
            await this.service.SendEmailsToEventGroups(id, emailHtmlContent);
            return this.RedirectToAction("EventDetails", new { id, messagesAreSend = GlobalConstants.ErrorMessages.MessagesAreSend });
        }
    }
}
