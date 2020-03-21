namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Data.Models;
    using QuizHut.Services.Events;
    using QuizHut.Services.Users;
    using QuizHut.Web.Infrastructure.Filters;
    using QuizHut.Web.ViewModels.Events;
    using QuizHut.Web.ViewModels.Students;
    using QuizHut.Web.ViewModels.Teachers;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class DashboardController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService service;
        private readonly IEventsService eventService;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            IUsersService service,
            IEventsService eventService)
        {
            this.userManager = userManager;
            this.service = service;
            this.eventService = eventService;
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

        public async Task<IActionResult> ResultsAll()
        {
            var events = await this.eventService.GetAllAsync<EventListViewModel>();
            var model = new EventsListAllViewModel
            {
                Events = events,
            };

            return this.View(model);
        }

        public async Task<IActionResult> EventsAll()
        {
            var events = await this.eventService.GetAllAsync<EventListViewModel>();
            var model = new EventsListAllViewModel { Events = events };
            return this.View(model);
        }

        public async Task<IActionResult> GroupsAll()
        {
            return this.View();
        }
        public async Task<IActionResult> QuizzesAll()
        {
            return this.View();
        }
        public async Task<IActionResult> StudentsAll()
        {
            return this.View();
        }
    }
}
