namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Data.Models;
    using QuizHut.Services.Events;
    using QuizHut.Services.Groups;
    using QuizHut.Services.Quizzes;
    using QuizHut.Services.Users;
    using QuizHut.Web.Infrastructure.Filters;
    using QuizHut.Web.Infrastructure.Helpers;
    using QuizHut.Web.ViewModels.Events;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Quizzes;
    using QuizHut.Web.ViewModels.Students;
    using QuizHut.Web.ViewModels.UsersInRole;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class DashboardController : AdministrationController
    {
        private const int PerPageDefaultValue = 5;
        private readonly IUsersService userService;
        private readonly IEventsService eventService;
        private readonly IGroupsService groupsService;
        private readonly IQuizzesService quizzesService;
        private readonly IDateTimeConverter dateTimeConverter;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public DashboardController(
            IUsersService userService,
            IEventsService eventService,
            IGroupsService groupsService,
            IQuizzesService quizzesService,
            IDateTimeConverter dateTimeConverter,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            this.userService = userService;
            this.eventService = eventService;
            this.groupsService = groupsService;
            this.quizzesService = quizzesService;
            this.dateTimeConverter = dateTimeConverter;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [ClearDashboardRequestInSessionActionFilterAttribute]
        public async Task<IActionResult> Index(string invalidEmail, string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var model = new DashboardIndexViewModel()
            {
                NewUser = new UserInputViewModel(),
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            ApplicationRole role = null;
            if (searchCriteria == GlobalConstants.AdministratorRoleName || searchCriteria == GlobalConstants.TeacherRoleName)
            {
                role = await this.roleManager.FindByNameAsync(searchCriteria);
            }

            var usersInRolesCount = await this.userService.GetAllInRolesCountAsync(searchCriteria, searchText, role?.Id);
            if (usersInRolesCount > 0)
            {
                var users = await this.userService.GetAllInRolesPerPageAsync<UserInRoleViewModel>(page, countPerPage, searchCriteria, searchText, role?.Id);
                foreach (var user in users)
                {
                    var appUser = await this.userManager.FindByIdAsync(user.Id);
                    var roles = await this.userManager.GetRolesAsync(appUser);
                    user.Role = string.Join(GlobalConstants.SplitOption, roles);
                }

                model.Users = users;
                model.PagesCount = (int)Math.Ceiling(usersInRolesCount / (decimal)countPerPage);
            }

            model.NewUser.IsNotAdded = invalidEmail != null ? true : false;
            model.NewUser.Email = invalidEmail ?? null;

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(UserInputViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Index", new { invalidEmail = GlobalConstants.Empty, roleName = model.RoleName });
            }

            var user = await this.userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return this.RedirectToAction("Index", new { invalidEmail = model.Email, roleName = model.RoleName });
            }

            await this.userManager.AddToRoleAsync(user, model.RoleName);
            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id, string roleName)
        {
            var user = await this.userManager.FindByIdAsync(id);
            await this.userManager.RemoveFromRoleAsync(user, roleName);
            return this.RedirectToAction("Index");
        }

        [SetDashboardRequestToTrueInSessionActionFilter]
        public async Task<IActionResult> ResultsAll(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var model = new EventsListAllViewModel<EventSimpleViewModel>()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            int allEventsCount = await this.eventService.GetAllEventsCountAsync(null, searchCriteria, searchText);

            if (allEventsCount > 0)
            {
                var events = await this.eventService.GetAllPerPage<EventSimpleViewModel>(page, countPerPage, null, searchCriteria, searchText);
                model.PagesCount = (int)Math.Ceiling(allEventsCount / (decimal)countPerPage);
                model.Events = events;
            }

            return this.View(model);
        }

        [SetDashboardRequestToTrueInSessionActionFilter]
        public async Task<IActionResult> EventsAll(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var model = new EventsListAllViewModel<EventListViewModel>()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            int allEventsCount = await this.eventService.GetAllEventsCountAsync(null, searchCriteria, searchText);

            if (allEventsCount > 0)
            {
                var events = await this.eventService.GetAllPerPage<EventListViewModel>(page, countPerPage, null, searchCriteria, searchText);
                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
                foreach (var @event in events)
                {
                    @event.Duration = this.dateTimeConverter.GetDurationString(@event.ActivationDateAndTime, @event.DurationOfActivity, timeZoneIana);
                    @event.StartDate = this.dateTimeConverter.GetDate(@event.ActivationDateAndTime, timeZoneIana);
                }

                model.PagesCount = (int)Math.Ceiling(allEventsCount / (decimal)countPerPage);
                model.Events = events;
            }

            return this.View(model);
        }

        [SetDashboardRequestToTrueInSessionActionFilter]
        public async Task<IActionResult> GroupsAll(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var model = new GroupsListAllViewModel()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allGroupsCount = await this.groupsService.GetAllGroupsCountAsync(null, searchCriteria, searchText);
            if (allGroupsCount > 0)
            {
                var groups = await this.groupsService.GetAllPerPageAsync<GroupListViewModel>(page, countPerPage, null, searchCriteria, searchText);
                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
                foreach (var group in groups)
                {
                    group.CreatedOnDate = this.dateTimeConverter.GetDate(group.CreatedOn, timeZoneIana);
                }

                model.Groups = groups;
                model.PagesCount = (int)Math.Ceiling(allGroupsCount / (decimal)countPerPage);
            }

            return this.View(model);
        }

        [SetDashboardRequestToTrueInSessionActionFilter]
        public async Task<IActionResult> QuizzesAll(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var model = new QuizzesAllListingViewModel()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allQuizzesCount = await this.quizzesService.GetAllQuizzesCountAsync(null, searchCriteria, searchText);
            if (allQuizzesCount > 0)
            {
                var quizzes = await this.quizzesService.GetAllPerPageAsync<QuizListViewModel>(page, countPerPage, null, searchCriteria, searchText);
                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
                foreach (var quiz in quizzes)
                {
                    quiz.CreatedOnDate = this.dateTimeConverter.GetDate(quiz.CreatedOn, timeZoneIana);
                }

                model.PagesCount = (int)Math.Ceiling(allQuizzesCount / (decimal)countPerPage);
                model.Quizzes = quizzes;
            }

            return this.View(model);
        }

        [SetDashboardRequestToTrueInSessionActionFilter]
        public async Task<IActionResult> StudentsAll(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var model = new StudentsAllViewModel()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allStudentsCount = await this.userService.GetAllStudentsCountAsync(null, searchCriteria, searchText);
            if (allStudentsCount > 0)
            {
                model.Students = await this.userService.GetAllStudentsPerPageAsync<StudentViewModel>(page, countPerPage, null, searchCriteria, searchText);
                model.PagesCount = (int)Math.Ceiling(allStudentsCount / (decimal)countPerPage);
            }

            return this.View(model);
        }
    }
}
