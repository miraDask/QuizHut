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
        private readonly UserManager<ApplicationUser> userManager;

        public DashboardController(
            IUsersService userService,
            IEventsService eventService,
            IGroupsService groupsService,
            IQuizzesService quizzesService,
            UserManager<ApplicationUser> userManager)
        {
            this.userService = userService;
            this.eventService = eventService;
            this.groupsService = groupsService;
            this.quizzesService = quizzesService;
            this.userManager = userManager;
        }

        [ClearDashboardRequestInSessionActionFilterAttribute]
        public IActionResult Index(string invalidEmail, string roleName)
        {
            if (invalidEmail != null)
            {
                return this.View(new InvalidUserEmailViewModel() { Email = invalidEmail, RoleName = roleName });
            }

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(UsersInRoleAllViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Index", new { invalidEmail = GlobalConstants.Empty, roleName = model.RoleName });
            }

            var user = await this.userManager.FindByEmailAsync(model.NewUser.Email);

            if (user == null)
            {
                return this.RedirectToAction("Index", new { invalidEmail = model.NewUser.Email, roleName = model.RoleName });
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
        public async Task<IActionResult> ResultsAll(int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var allEventsCount = this.eventService.GetAllEventsCount();
            int pagesCount = 0;
            var model = new EventsListAllViewModel()
            {
                CurrentPage = page,
                PagesCount = pagesCount,
            };

            if (allEventsCount > 0)
            {
                pagesCount = (int)Math.Ceiling(allEventsCount / (decimal)countPerPage);
                var events = await this.eventService.GetAllPerPage<EventListViewModel>(page, countPerPage);
                model.PagesCount = pagesCount;
                model.Events = events;
            }

            return this.View(model);
        }

        [SetDashboardRequestToTrueInSessionActionFilter]
        public async Task<IActionResult> EventsAll(int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var allEventsCount = this.eventService.GetAllEventsCount();
            int pagesCount = 0;
            var model = new EventsListAllViewModel()
            {
                CurrentPage = page,
                PagesCount = pagesCount,
            };

            if (allEventsCount > 0)
            {
                pagesCount = (int)Math.Ceiling(allEventsCount / (decimal)countPerPage);
                var events = await this.eventService.GetAllPerPage<EventListViewModel>(page, countPerPage);
                model.PagesCount = pagesCount;
                model.Events = events;
            }

            return this.View(model);
        }

        [SetDashboardRequestToTrueInSessionActionFilter]
        public async Task<IActionResult> GroupsAll(int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var allGroupsCount = this.groupsService.GetAllGroupsCount();
            int pagesCount = 0;
            var model = new GroupsListAllViewModel()
            {
                CurrentPage = page,
                PagesCount = pagesCount,
            };

            if (allGroupsCount > 0)
            {
                pagesCount = (int)Math.Ceiling(allGroupsCount / (decimal)countPerPage);
                var groups = await this.groupsService.GetAllPerPageAsync<GroupListViewModel>(page, countPerPage);
                model.Groups = groups;
                model.PagesCount = pagesCount;
            }

            return this.View(model);
        }

        [SetDashboardRequestToTrueInSessionActionFilter]
        public async Task<IActionResult> QuizzesAll(int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var allQuizzesCount = this.quizzesService.GetAllQuizzesCount();
            int pagesCount = 0;
            var model = new QuizzesAllListingViewModel()
            {
                CurrentPage = page,
                PagesCount = pagesCount,
            };

            if (allQuizzesCount > 0)
            {
                pagesCount = (int)Math.Ceiling(allQuizzesCount / (decimal)countPerPage);
                var quizzes = await this.quizzesService.GetAllPerPage<QuizListViewModel>(page, countPerPage);
                model.PagesCount = pagesCount;
                model.Quizzes = quizzes;
            }

            return this.View(model);
        }

        [SetDashboardRequestToTrueInSessionActionFilter]
        public async Task<IActionResult> StudentsAll(int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var allStudentsCount = this.userService.GetAllStudentsCount();
            int pagesCount = 0;
            var model = new StudentsAllViewModel()
            {
                CurrentPage = page,
                PagesCount = pagesCount,
            };

            if (allStudentsCount > 0)
            {
                pagesCount = (int)Math.Ceiling(allStudentsCount / (decimal)countPerPage);
                var students = await this.userService.GetAllStudentsPerPageAsync<StudentViewModel>(page, countPerPage);
                model.Students = students;
                model.PagesCount = pagesCount;
            }

            return this.View(model);
        }
    }
}
