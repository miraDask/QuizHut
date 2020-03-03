namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Data.Models;
    using QuizHut.Services.Data;
    using QuizHut.Services.Users;
    using QuizHut.Web.Filters;
    using QuizHut.Web.ViewModels.Moderators;
    using QuizHut.Web.ViewModels.Participants;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService settingsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService service;

        public DashboardController(ISettingsService settingsService, UserManager<ApplicationUser> userManager, IUsersService service)
        {
            this.settingsService = settingsService;
            this.userManager = userManager;
            this.service = service;
        }

        public async Task<IActionResult> Index(string invalidEmail)
        {
            //var viewModel = new IndexViewModel { SettingsCount = this.settingsService.GetCount(), };
            var teachers = await this.service.GetAllByRoleAsync<ModeratorViewModel>(GlobalConstants.TeacherRoleName);
            var model = new ModeratorsAllViewModel()
            {
                Moderators = teachers,
                NewModerator = new ParticipantInputViewModel(),
            };

            if (invalidEmail != null)
            {
                model.NewModerator.IsNotAdded = true;
                model.NewModerator.Email = invalidEmail;
            }

            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> Index(ModeratorsAllViewModel model)
        {
            var isAdded = await this.service.AssignRoleAsync(model.NewModerator.Email, GlobalConstants.TeacherRoleName);

            if (!isAdded)
            {
                return this.RedirectToAction("Index", new { invalidEmail = model.NewModerator.Email });
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
