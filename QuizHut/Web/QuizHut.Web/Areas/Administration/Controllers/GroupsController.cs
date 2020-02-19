namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Group;
    using QuizHut.Web.ViewModels.Groups;

    public class GroupsController : AdministrationController
    {
        private readonly IGroupService service;
        private readonly UserManager<ApplicationUser> userManager;

        public GroupsController(IGroupService service, UserManager<ApplicationUser> userManager)
        {
            this.service = service;
            this.userManager = userManager;
        }

        public async Task<IActionResult> AllGroupsCreatedByUser()
        {
            var userId = this.userManager.GetUserId(this.User);
            var groups = await this.service.GetAllByCreatorIdAsync<GroupListViewModel>(userId);
            var model = new GroupsListAllViewModel() { Groups = groups };
            return this.View(model);
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGroupInputViewModel model)
        {
            var userId = this.userManager.GetUserId(this.User);
            var groupId = await this.service.CreateAsync(model.Name, userId);

            return this.RedirectToAction("AssignQuiz", new { id = groupId });
        }

        public IActionResult AssignQuiz(string id)
        {
            return this.View();
        }
    }
}