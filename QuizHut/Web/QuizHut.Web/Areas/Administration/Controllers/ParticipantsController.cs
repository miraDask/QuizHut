namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.User;
    using QuizHut.Web.ViewModels.Participants;

    public class ParticipantsController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserService service;

        public ParticipantsController(UserManager<ApplicationUser> userManager, IUserService service)
        {
            this.userManager = userManager;
            this.service = service;
        }

        public async Task<IActionResult> AllParticipantsAddedByUser()
        {
            var userId = this.userManager.GetUserId(this.User);
            var participants = await this.service.GetAllByUserIdAsync<ParticipantViewModel>(userId);
            var model = new AllParticipantsAddedByUserViewModel()
            {
                Participants = participants,
                NewParticipant = new ParticipantInputViewModel(),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AllParticipantsAddedByUser(AllParticipantsAddedByUserViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var userId = this.userManager.GetUserId(this.User);
            var partisipantIsAdded = await this.service.AddAsync(model.NewParticipant.Email, userId);

            if (!partisipantIsAdded)
            {
                model.NewParticipant.IsNotAdded = true;
                return this.View(model);
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
