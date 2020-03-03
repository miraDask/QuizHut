namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Users;
    using QuizHut.Web.Filters;
    using QuizHut.Web.ViewModels.Participants;

    public class ParticipantsController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService service;

        public ParticipantsController(UserManager<ApplicationUser> userManager, IUsersService service)
        {
            this.userManager = userManager;
            this.service = service;
        }

        public async Task<IActionResult> AllParticipantsAddedByUser(string invalidEmail)
        {
            var userId = this.userManager.GetUserId(this.User);
            var participants = await this.service.GetAllByUserIdAsync<ParticipantViewModel>(userId);
            var model = new AllParticipantsAddedByUserViewModel()
            {
                Participants = participants,
                NewParticipant = new ParticipantInputViewModel(),
            };

            if (invalidEmail != null)
            {
                model.NewParticipant.IsNotAdded = true;
                model.NewParticipant.Email = invalidEmail;
            }

            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AllParticipantsAddedByUser(AllParticipantsAddedByUserViewModel model)
        {
            var userId = this.userManager.GetUserId(this.User);
            var partisipantIsAdded = await this.service.AddAsync(model.NewParticipant.Email, userId);

            if (!partisipantIsAdded)
            {
                return this.RedirectToAction("AllParticipantsAddedByUser", new { invalidEmail = model.NewParticipant.Email });
            }

            return this.RedirectToAction("AllParticipantsAddedByUser");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            await this.service.DeleteAsync(id, userId);
            return this.RedirectToAction("AllParticipantsAddedByUser");
        }
    }
}
