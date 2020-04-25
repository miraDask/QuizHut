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
    using QuizHut.Services.Users;
    using QuizHut.Web.Infrastructure.Filters;
    using QuizHut.Web.Infrastructure.Helpers;
    using QuizHut.Web.ViewModels.Events;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Students;

    public class GroupsController : AdministrationController
    {
        private const int PerPageDefaultValue = 5;
        private readonly IGroupsService service;
        private readonly IEventsService eventService;
        private readonly IUsersService userService;
        private readonly IDateTimeConverter dateTimeConverter;
        private readonly UserManager<ApplicationUser> userManager;

        public GroupsController(
            IGroupsService service,
            IEventsService eventService,
            IUsersService userService,
            IDateTimeConverter dateTimeConverter,
            UserManager<ApplicationUser> userManager)
        {
            this.service = service;
            this.eventService = eventService;
            this.userService = userService;
            this.dateTimeConverter = dateTimeConverter;
            this.userManager = userManager;
        }

        [ClearDashboardRequestInSessionActionFilterAttribute]
        public async Task<IActionResult> AllGroupsCreatedByTeacher(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var userId = this.userManager.GetUserId(this.User);

            var model = new GroupsListAllViewModel()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allGroupsCreatedByTeacherCount = await this.service.GetAllGroupsCountAsync(userId, searchCriteria, searchText);
            if (allGroupsCreatedByTeacherCount > 0)
            {
                var groups = await this.service.GetAllPerPageAsync<GroupListViewModel>(page, countPerPage, userId, searchCriteria, searchText);
                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
                foreach (var group in groups)
                {
                    group.CreatedOnDate = this.dateTimeConverter.GetDate(group.CreatedOn, timeZoneIana);
                }

                model.Groups = groups;
                model.PagesCount = (int)Math.Ceiling(allGroupsCreatedByTeacherCount / (decimal)countPerPage);
            }

            return this.View(model);
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> Create(CreateGroupInputViewModel model)
        {
            var userId = this.userManager.GetUserId(this.User);
            var groupId = await this.service.CreateGroupAsync(model.Name, userId);

            return this.RedirectToAction("AssignEvent", new { id = groupId });
        }

        public async Task<IActionResult> AssignEvent(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var events = await this.eventService.GetAllFiteredByStatusAndGroupAsync<EventsAssignViewModel>(Status.Ended, id, userId);
            var model = await this.service.GetGroupModelAsync<GroupWithEventsViewModel>(id);
            model.Events = events;

            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AssignEvent(GroupWithEventsViewModel model)
        {
            var eventsIds = model.Events.Where(x => x.IsAssigned).Select(x => x.Id).ToList();

            if (eventsIds.Count() == 0)
            {
                model.Error = true;
                return this.View(model);
            }

            await this.service.AssignEventsToGroupAsync(model.Id, eventsIds);
            return this.RedirectToAction("AssignStudents", new { id = model.Id });
        }

        public async Task<IActionResult> AssignStudents(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var students = await this.userService.GetAllStudentsAsync<StudentViewModel>(userId);
            var model = new GroupWithStudentsViewModel() { Id = id, Students = students };
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AssignStudents(GroupWithStudentsViewModel model)
        {
            var studentsIds = model.Students.Where(x => x.IsAssigned).Select(x => x.Id).ToList();

            if (studentsIds.Count() == 0)
            {
                model.Error = true;
                return this.View(model);
            }

            await this.service.AssignStudentsToGroupAsync(model.Id, studentsIds);
            return this.RedirectToAction("GroupDetails", new { id = model.Id });
        }

        [HttpGet]
        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public async Task<IActionResult> GroupDetails(string id)
        {
            var events = await this.eventService.GetAllByGroupIdAsync<EventsAssignViewModel>(id);
            var students = await this.userService.GetAllByGroupIdAsync<StudentViewModel>(id);
            var model = await this.service.GetGroupModelAsync<GroupDetailsViewModel>(id);
            model.Students = students;
            model.Events = events;

            return this.View(model);
        }

        public IActionResult Edit(string id)
        {
            return this.RedirectToAction("GroupDetails", new { id });
        }

        public async Task<IActionResult> Delete(string id)
        {
            await this.service.DeleteAsync(id);
            return this.RedirectToAction("AllGroupsCreatedByTeacher");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEventFromGroup(string groupId, string eventId)
        {
            await this.service.DeleteEventFromGroupAsync(groupId, eventId);
            return this.RedirectToAction("GroupDetails", new { id = groupId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudentFromGroup(string groupId, string studentId)
        {
            await this.service.DeleteStudentFromGroupAsync(groupId, studentId);
            return this.RedirectToAction("GroupDetails", new { id = groupId });
        }

        [HttpGet]
        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public async Task<IActionResult> AddNewEvent(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            IList<EventsAssignViewModel> events;
            var isDashboardRequest = this.HttpContext.Session.GetString(GlobalConstants.DashboardRequest) != null;
            if (isDashboardRequest)
            {
                events = await this.eventService.GetAllFiteredByStatusAndGroupAsync<EventsAssignViewModel>(Status.Ended, id);
            }
            else
            {
                events = await this.eventService.GetAllFiteredByStatusAndGroupAsync<EventsAssignViewModel>(Status.Ended, id, userId);
            }

            var model = await this.service.GetGroupModelAsync<GroupWithEventsViewModel>(id);
            model.Events = events;
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AddNewEvent(GroupWithEventsViewModel model)
        {
            var eventsIds = model.Events.Where(x => x.IsAssigned).Select(x => x.Id).ToList();

            if (eventsIds.Count() == 0)
            {
                model.Error = true;
                return this.View(model);
            }

            await this.service.AssignEventsToGroupAsync(model.Id, eventsIds);
            return this.RedirectToAction("GroupDetails", new { id = model.Id });
        }

        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public async Task<IActionResult> AddStudents(string id)
        {
            IList<StudentViewModel> students;
            var isDashboardRequest = this.HttpContext.Session.GetString(GlobalConstants.DashboardRequest) != null;
            if (isDashboardRequest)
            {
                students = await this.userService.GetAllStudentsAsync<StudentViewModel>(null, id);
            }
            else
            {
                var userId = this.userManager.GetUserId(this.User);
                students = await this.userService.GetAllStudentsAsync<StudentViewModel>(userId, id);
            }

            var model = new GroupWithStudentsViewModel() { Id = id, Students = students };
            model.Students = students;
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AddStudents(GroupWithStudentsViewModel model)
        {
            var studentsIds = model.Students.Where(x => x.IsAssigned).Select(x => x.Id).ToList();

            if (studentsIds.Count() == 0)
            {
                model.Error = true;
                return this.View(model);
            }

            await this.service.AssignStudentsToGroupAsync(model.Id, studentsIds);
            return this.RedirectToAction("GroupDetails", new { id = model.Id });
        }

        [HttpGet]
        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public IActionResult EditName(string id, string name)
        {
            var model = new EditGroupNameInputViewModel() { Id = id, Name = name };
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> EditName(EditGroupNameInputViewModel model)
        {
            await this.service.UpdateNameAsync(model.Id, model.Name);
            return this.RedirectToAction("GroupDetails", new { id = model.Id });
        }
    }
}
