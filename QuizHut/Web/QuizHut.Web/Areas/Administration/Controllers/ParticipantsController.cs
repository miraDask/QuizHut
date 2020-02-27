namespace QuizHut.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.User;
    using QuizHut.Web.ViewModels.Participants;
    using System.Threading.Tasks;

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
            var model = new AllParticipantsAddedByUserViewModel() { Participants = participants };
            return this.View(model);
        }

        public async Task<IActionResult> Add(ParticipantInputViewModel model)
        {
            var userId = this.userManager.GetUserId(this.User);
            var partisipantIsAded = await this.service.AddAsync(model.Email, userId);

            // if participantIsAded == false -> display error : participant doesn't exist

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
