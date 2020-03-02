namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Group;
    using QuizHut.Services.Quiz;
    using QuizHut.Services.User;
    using QuizHut.Web.Filters;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Participants;
    using QuizHut.Web.ViewModels.Quizzes;

    public class GroupsController : AdministrationController
    {
        private readonly IGroupService service;
        private readonly IQuizService quizService;
        private readonly IUserService userService;
        private readonly UserManager<ApplicationUser> userManager;

        public GroupsController(
            IGroupService service,
            IQuizService quizService,
            IUserService userService,
            UserManager<ApplicationUser> userManager)
        {
            this.service = service;
            this.quizService = quizService;
            this.userService = userService;
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
        [ModelStateValidationActionFilterAttribute]
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
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AssignQuiz(GroupWithQuizzesViewModel model)
        {
            var quizzesIds = model.Quizzes.Where(x => x.IsAssigned).Select(x => x.Id).ToList();
            await this.service.AssignQuizzesToGroupAsync(model.GroupId, quizzesIds);
            return this.RedirectToAction("AssignParticipants", new { id = model.GroupId });
        }

        public async Task<IActionResult> AssignParticipants(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var participants = await this.userService.GetAllByUserIdAsync<ParticipantViewModel>(userId);
            var model = new GroupWithParticipantsViewModel() { GroupId = id, Participants = participants };
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AssignParticipants(GroupWithParticipantsViewModel model)
        {
            var participantsIds = model.Participants.Where(x => x.IsAssigned).Select(x => x.Id).ToList();
            await this.service.AssignParticipantsToGroupAsync(model.GroupId, participantsIds);
            return this.RedirectToAction("GroupDetails", new { id = model.GroupId });
        }

        [HttpGet]
        public async Task<IActionResult> GroupDetails(string id)
        {
            var model = await this.service.GetGroupDetailsModelAsync(id);
            return this.View(model);
        }

        [HttpPost]
        public IActionResult Edit(string id)
        {
            return this.RedirectToAction("GroupDetails", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await this.service.DeleteAsync(id);
            return this.RedirectToAction("AllGroupsCreatedByUser");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuizFromGroup(string groupId, string quizId)
        {
            await this.service.DeleteQuizFromGroupAsync(groupId, quizId);
            return this.RedirectToAction("GroupDetails", new { id = groupId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteParticipantFromGroup(string groupId, string participantId)
        {
            await this.service.DeleteParticipantFromGroupAsync(groupId, participantId);
            return this.RedirectToAction("GroupDetails", new { id = groupId });
        }

        public async Task<IActionResult> AddNewQuiz(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var quizzes = await this.quizService.GetAllAsync<QuizAssignViewModel>();
            quizzes = await this.service.FilterQuizzesThatAreNotAssignedToThisGroup(id, quizzes);
            var model = await this.service.GetGroupModelAsync(id, userId, quizzes);

            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AddNewQuiz(GroupWithQuizzesViewModel model)
        {
            var quizzesIds = model.Quizzes.Where(x => x.IsAssigned).Select(x => x.Id).ToList();
            await this.service.AssignQuizzesToGroupAsync(model.GroupId, quizzesIds);
            return this.RedirectToAction("GroupDetails", new { id = model.GroupId });
        }

        public async Task<IActionResult> AddParticipants(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var participants = await this.userService.GetAllByUserIdAsync<ParticipantViewModel>(userId);
            participants = await this.service.FilterParticipantsThatAreNotAssignedToThisGroup(id, participants);
            var model = new GroupWithParticipantsViewModel() { GroupId = id, Participants = participants };
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AddParticipants(GroupWithParticipantsViewModel model)
        {
            var participantsIds = model.Participants.Where(x => x.IsAssigned).Select(x => x.Id).ToList();
            await this.service.AssignParticipantsToGroupAsync(model.GroupId, participantsIds);
            return this.RedirectToAction("GroupDetails", new { id = model.GroupId });
        }

        public IActionResult EditName(string id, string name)
        {
            var model = new EditGroupNameInputViewModel() { Id = id, Name = name };
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> EditName(EditGroupNameInputViewModel model)
        {
            await this.service.UpdateNameAsync(model.Id, model.Name);
            return this.RedirectToAction("GroupDetails", new { id = model.Id });
        }
    }
}