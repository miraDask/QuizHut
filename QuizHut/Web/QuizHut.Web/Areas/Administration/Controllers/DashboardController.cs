namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Data.Models;
    using QuizHut.Services.Users;
    using QuizHut.Web.Filters;
    using QuizHut.Web.ViewModels.Students;
    using QuizHut.Web.ViewModels.Teachers;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class DashboardController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService service;

        public DashboardController(UserManager<ApplicationUser> userManager, IUsersService service)
        {
            this.userManager = userManager;
            this.service = service;
        }

        public async Task<IActionResult> Index(string invalidEmail)
        {
            // var viewModel = new IndexViewModel { SettingsCount = this.settingsService.GetCount(), };
            var teachers = await this.service.GetAllByRoleAsync<TeacherViewModel>(GlobalConstants.TeacherRoleName);
            var model = new TeachersAllViewModel()
            {
                Teachers = teachers,
                NewTeacher = new StudentInputViewModel(),
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
    }
}
