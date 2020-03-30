namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
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
        private readonly IUsersService service;
        private readonly IEventsService eventService;
        private readonly IGroupsService groupsService;
        private readonly IQuizzesService quizzesService;

        public DashboardController(
            IUsersService service,
            IEventsService eventService,
            IGroupsService groupsService,
            IQuizzesService quizzesService)
        {
            this.service = service;
            this.eventService = eventService;
            this.groupsService = groupsService;
            this.quizzesService = quizzesService;
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

            var isAdded = await this.service.AssignRoleAsync(model.NewUser.Email, model.RoleName);

            if (!isAdded)
            {
                return this.RedirectToAction("Index", new { invalidEmail = model.NewUser.Email, roleName = model.RoleName });
            }

            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id, string roleName)
        {
            await this.service.RemoveFromRoleAsync(id, roleName);
            return this.RedirectToAction("Index");
        }

        [ClearDashboardRequestInSessionActionFilterAttribute]
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
            //var events = await this.eventService.GetAllPerPage<EventListViewModel>(page, countPerPage);
            //var model = new EventsListAllViewModel { Events = events };
            //return this.View(model);
        }

        [SetDashboardRequestToTrueInSessionActionFilter]
        public async Task<IActionResult> GroupsAll()
        {
            var groups = await this.groupsService.GetAllAsync<GroupListViewModel>();
            var model = new GroupsListAllViewModel() { Groups = groups };
            return this.View(model);
        }

        [ClearDashboardRequestInSessionActionFilterAttribute]
        public async Task<IActionResult> QuizzesAll()
        {
            var quizzes = await this.quizzesService.GetAllAsync<QuizListViewModel>(false);
            var model = new QuizzesAllListingViewModel() { Quizzes = quizzes };
            return this.View(model);
        }

        [ClearDashboardRequestInSessionActionFilterAttribute]
        public async Task<IActionResult> StudentsAll()
        {
            var students = await this.service.GetAllByUserIdAsync<StudentViewModel>();
            return this.View(students);
        }
    }
}
