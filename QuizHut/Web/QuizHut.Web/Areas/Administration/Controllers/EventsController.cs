namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using QuizHut.Common;
    using QuizHut.Common.Hubs;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Services.Events;
    using QuizHut.Services.Groups;
    using QuizHut.Services.Quizzes;
    using QuizHut.Web.Infrastructure.Filters;
    using QuizHut.Web.Infrastructure.Helpers;
    using QuizHut.Web.ViewModels.Events;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Quizzes;

    public class EventsController : AdministrationController
    {
        private const int PerPageDefaultValue = 5;
        private readonly IEventsService service;
        private readonly IQuizzesService quizService;
        private readonly IGroupsService groupsService;
        private readonly IDateTimeConverter dateTimeConverter;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<QuizHub> hub;

        public EventsController(
            IEventsService service,
            IQuizzesService quizService,
            IGroupsService groupsService,
            IDateTimeConverter dateTimeConverter,
            UserManager<ApplicationUser> userManager,
            IHubContext<QuizHub> hub)
        {
            this.service = service;
            this.quizService = quizService;
            this.groupsService = groupsService;
            this.dateTimeConverter = dateTimeConverter;
            this.userManager = userManager;
            this.hub = hub;
        }

        [ClearDashboardRequestInSessionActionFilterAttribute]
        public async Task<IActionResult> AllEventsCreatedByTeacher(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var userId = this.userManager.GetUserId(this.User);
            int pagesCount = 0;

            var model = new EventsListAllViewModel<EventListViewModel>()
            {
                CurrentPage = page,
                PagesCount = pagesCount,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allEventsCreatedByTeacher = await this.service.GetAllEventsCountAsync(userId, searchCriteria, searchText);
            if (allEventsCreatedByTeacher > 0)
            {
                pagesCount = (int)Math.Ceiling(allEventsCreatedByTeacher / (decimal)countPerPage);
                var events = await this.service.GetAllPerPage<EventListViewModel>(page, countPerPage, userId, searchCriteria, searchText);
                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
                foreach (var @event in events)
                {
                    @event.Duration = this.dateTimeConverter.GetDurationString(@event.ActivationDateAndTime, @event.DurationOfActivity, timeZoneIana);
                    @event.StartDate = this.dateTimeConverter.GetDate(@event.ActivationDateAndTime, timeZoneIana);
                }

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
            var timeErrorMessage = this.service.GetTimeErrorMessage(model.ActiveFrom, model.ActiveTo, model.ActivationDate, model.TimeZone);
            if (timeErrorMessage != null)
            {
                model.Error = timeErrorMessage;
                return this.View(model);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            var eventId = await this.service.CreateEventAsync(model.Name, model.ActivationDate, model.ActiveFrom, model.ActiveTo, user.Id);
            await this.hub.Clients.Group(GlobalConstants.AdministratorRoleName).SendAsync("NewEventUpdate", user.UserName, model.Name);

            return this.RedirectToAction("AssignGroupsToEvent", new { id = eventId });
        }

        public async Task<IActionResult> AssignGroupsToEvent(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var groups = await this.groupsService.GetAllAsync<GroupAssignViewModel>(userId);
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

            await this.service.AssignGroupsToEventAsync(groupIds, model.Id);
            return this.RedirectToAction("AssignQuizToEvent", new { id = model.Id });
        }

        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public async Task<IActionResult> AddGroupsToEvent(string id)
        {
            var userId = this.userManager.GetUserId(this.User);

            IList<GroupAssignViewModel> groups;
            var isDashboardRequest = this.HttpContext.Session.GetString(GlobalConstants.DashboardRequest) != null;

            if (isDashboardRequest)
            {
                groups = await this.groupsService.GetAllAsync<GroupAssignViewModel>(null, id);
            }
            else
            {
                groups = await this.groupsService.GetAllAsync<GroupAssignViewModel>(userId, id);
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

        [HttpGet]
        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public async Task<IActionResult> AssignQuizToEvent(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            IList<QuizAssignViewModel> quizzes;
            var isDashboardRequest = this.HttpContext.Session.GetString(GlobalConstants.DashboardRequest) != null;
            if (isDashboardRequest)
            {
                quizzes = await this.quizService.GetAllUnAssignedToEventAsync<QuizAssignViewModel>();
            }
            else
            {
                quizzes = await this.quizService.GetAllUnAssignedToEventAsync<QuizAssignViewModel>(userId);
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

            await this.service.AssigQuizToEventAsync(model.Id, quizzes[0].Id, model.TimeZone);
            return this.RedirectToAction("EventDetails", new { id = model.Id });
        }

        [HttpGet]
        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public async Task<IActionResult> EventDetails(string id, string messagesAreSend)
        {
            var groups = await this.groupsService.GetAllByEventIdAsync<GroupAssignViewModel>(id);
            var model = await this.service.GetEventModelByIdAsync<EventDetailsViewModel>(id);
            var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
            var duration = this.dateTimeConverter.GetDurationString(model.ActivationDateAndTime, model.DurationOfActivity, timeZoneIana).Split(" - ");
            model.ActivationDate = this.dateTimeConverter.GetDate(model.ActivationDateAndTime, timeZoneIana);
            model.ActiveFrom = duration[0];
            model.ActiveTo = duration[1];
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

        public async Task<IActionResult> ActiveEventsAll(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var userId = this.userManager.GetUserId(this.User);

            var model = new EventsListAllViewModel<EventSimpleViewModel>()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allActiveEventsCreatedByTeacher = await this.service.GetEventsCountByCreatorIdAndStatusAsync(Status.Active, userId, searchCriteria, searchText);
            if (allActiveEventsCreatedByTeacher > 0)
            {
                var events = await this.service
                    .GetAllPerPageByCreatorIdAndStatus<EventSimpleViewModel>(page, countPerPage, Status.Active, userId, searchCriteria, searchText);
                model.Events = events;
                model.PagesCount = (int)Math.Ceiling(allActiveEventsCreatedByTeacher / (decimal)countPerPage);
            }

            return this.View(model);
        }

        public async Task<IActionResult> EndedEventsAll(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var userId = this.userManager.GetUserId(this.User);

            var model = new EventsListAllViewModel<EventSimpleViewModel>()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allEndedEventsCreatedByTeacher = await this.service.GetEventsCountByCreatorIdAndStatusAsync(Status.Ended, userId, searchCriteria, searchText);
            if (allEndedEventsCreatedByTeacher > 0)
            {
                var events = await this.service.GetAllPerPageByCreatorIdAndStatus<EventSimpleViewModel>(page, countPerPage, Status.Ended, userId, searchCriteria, searchText);
                model.Events = events;
                model.PagesCount = (int)Math.Ceiling(allEndedEventsCreatedByTeacher / (decimal)countPerPage);
            }

            return this.View(model);
        }

        [HttpGet]
        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public async Task<IActionResult> EditEventDetails(string id)
        {
            var editModel = await this.service.GetEventModelByIdAsync<EditEventDetailsInputViewModel>(id);

            return this.View(editModel);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> EditEventDetails(EditEventDetailsInputViewModel model)
        {
            var timeErrorMessage = this.service.GetTimeErrorMessage(model.ActiveFrom, model.ActiveTo, model.ActivationDate, model.TimeZone);
            if (timeErrorMessage != null)
            {
                model.Error = timeErrorMessage;
                return this.View(model);
            }

            await this.service.UpdateAsync(model.Id, model.Name, model.ActivationDate, model.ActiveFrom, model.ActiveTo, model.TimeZone);

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
