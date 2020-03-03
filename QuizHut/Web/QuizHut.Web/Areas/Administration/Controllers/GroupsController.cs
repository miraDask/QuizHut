namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Groups;
    using QuizHut.Services.Quizzes;
    using QuizHut.Services.Users;
    using QuizHut.Web.Filters;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Quizzes;
    using QuizHut.Web.ViewModels.Students;

    public class GroupsController : AdministrationController
    {
        private readonly IGroupsService service;
        private readonly IQuizzesService quizService;
        private readonly IUsersService userService;
        private readonly UserManager<ApplicationUser> userManager;

        public GroupsController(
            IGroupsService service,
            IQuizzesService quizService,
            IUsersService userService,
            UserManager<ApplicationUser> userManager)
        {
            this.service = service;
            this.quizService = quizService;
            this.userService = userService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> AllGroupsCreatedByTeacher()
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
            return this.RedirectToAction("AssignStudents", new { id = model.GroupId });
        }

        public async Task<IActionResult> AssignStudents(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var students = await this.userService.GetAllByUserIdAsync<StudentViewModel>(userId);
            var model = new GroupWithStudentsViewModel() { GroupId = id, Students = students };
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AssignStudents(GroupWithStudentsViewModel model)
        {
            var studentsIds = model.Students.Where(x => x.IsAssigned).Select(x => x.Id).ToList();
            await this.service.AssignStudentsToGroupAsync(model.GroupId, studentsIds);
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
            return this.RedirectToAction("AllGroupsCreatedByTeacher");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuizFromGroup(string groupId, string quizId)
        {
            await this.service.DeleteQuizFromGroupAsync(groupId, quizId);
            return this.RedirectToAction("GroupDetails", new { id = groupId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudentFromGroup(string groupId, string studentId)
        {
            await this.service.DeleteStudentFromGroupAsync(groupId, studentId);
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

        public async Task<IActionResult> AddStudents(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var students = await this.userService.GetAllByUserIdAsync<StudentViewModel>(userId);
            students = await this.service.FilterStudentsThatAreNotAssignedToThisGroup(id, students);
            var model = new GroupWithStudentsViewModel() { GroupId = id, Students = students };
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AddStudents(GroupWithStudentsViewModel model)
        {
            var studentsIds = model.Students.Where(x => x.IsAssigned).Select(x => x.Id).ToList();
            await this.service.AssignStudentsToGroupAsync(model.GroupId, studentsIds);
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