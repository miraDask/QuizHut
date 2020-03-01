namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Data.Models;
    using QuizHut.Services.Data;
    using QuizHut.Services.User;
    using QuizHut.Web.ViewModels.Moderators;
    using QuizHut.Web.ViewModels.Participants;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService settingsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserService service;

        public DashboardController(ISettingsService settingsService, UserManager<ApplicationUser> userManager, IUserService service)
        {
            this.settingsService = settingsService;
            this.userManager = userManager;
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            //var viewModel = new IndexViewModel { SettingsCount = this.settingsService.GetCount(), };
            var moderators = await this.service.GetAllByRoleAsync<ModeratorViewModel>(GlobalConstants.ModeratorRoleName);
            var model = new ModeratorsAllViewModel()
            {
                Moderators = moderators,
                NewModerator = new ParticipantInputViewModel(),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ModeratorsAllViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var isAdded = await this.service.AssignRoleAsync(model.NewModerator.Email, GlobalConstants.ModeratorRoleName);

            if (!isAdded)
            {
                model.NewModerator.IsNotAdded = true;
                return this.View(model);
            }

            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            await this.service.RemoveFromRoleAsync(id, GlobalConstants.ModeratorRoleName);
            return this.RedirectToAction("Index");
        }
    }
}
