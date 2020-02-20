namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Group;
    using QuizHut.Services.Quiz;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Quizzes;

    public class GroupsController : AdministrationController
    {
        private readonly IGroupService service;
        private readonly IQuizService quizService;
        private readonly UserManager<ApplicationUser> userManager;

        public GroupsController(
            IGroupService service,
            IQuizService quizService,
            UserManager<ApplicationUser> userManager)
        {
            this.service = service;
            this.quizService = quizService;
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

        public async Task<IActionResult> AssignQuiz(string id)
        {

            var userId = this.userManager.GetUserId(this.User);
            var quizzes = await this.quizService.GetAllAsync<QuizAssignViewModel>();
            var model = await this.service.GetGroupModelAsync(id, userId, quizzes);

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignQuiz(GroupWithQuizzesViewModel model)
        {
            var quizzesIds = model.Quizzes.Where(x => x.IsAssigned).Select(x => x.Id).ToList();
            await this.service.AssignQuizzesToGroup(model.GroupId, quizzesIds);
            return this.RedirectToAction("AssignParticipantsToGroup", new { id = model.GroupId });
        }

        public async Task<IActionResult> AssignParticipants(string id)
        {
            return this.View();
        }
    }
}