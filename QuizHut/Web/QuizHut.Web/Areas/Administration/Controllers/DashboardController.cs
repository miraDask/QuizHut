namespace QuizHut.Web.Areas.Administration.Controllers
{
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
    using QuizHut.Web.ViewModels.Teachers;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class DashboardController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService service;
        private readonly IEventsService eventService;
        private readonly IGroupsService groupsService;
        private readonly IQuizzesService quizzesService;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            IUsersService service,
            IEventsService eventService,
            IGroupsService groupsService,
            IQuizzesService quizzesService)
        {
            this.userManager = userManager;
            this.service = service;
            this.eventService = eventService;
            this.groupsService = groupsService;
            this.quizzesService = quizzesService;
        }

        public async Task<IActionResult> Index(string invalidEmail)
        {
            var teachers = await this.service.GetAllByRoleAsync<TeacherViewModel>(GlobalConstants.TeacherRoleName);
            var model = new TeachersAllViewModel()
            {
                Teachers = teachers,
                NewTeacher = new UserInputViewModel(),
            };

            if (invalidEmail != null)
            {
                model.NewTeacher.IsNotAdded = true;
                model.NewTeacher.Email = invalidEmail;
            }

            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        [ClearDashboardRequestInSessionActionFilterAttribute]
        public async Task<IActionResult> Index(TeachersAllViewModel model)
        {
            var isAdded = await this.service.AssignRoleAsync(model.NewTeacher.Email, GlobalConstants.TeacherRoleName);

            if (!isAdded)
            {
                return this.RedirectToAction("Index", new { invalidEmail = model.NewTeacher.Email });
            }

            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            await this.service.RemoveFromRoleAsync(id, GlobalConstants.TeacherRoleName);
            return this.RedirectToAction("Index");
        }

        [ClearDashboardRequestInSessionActionFilterAttribute]
        public async Task<IActionResult> ResultsAll()
        {
            var events = await this.eventService.GetAllAsync<EventListViewModel>();
            var model = new EventsListAllViewModel
            {
                Events = events,
            };

            return this.View(model);
        }

        [SetDashboardRequestToTrueInSessionActionFilter]
        public async Task<IActionResult> EventsAll()
        {
            var events = await this.eventService.GetAllAsync<EventListViewModel>();
            var model = new EventsListAllViewModel { Events = events };
            return this.View(model);
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
