namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Category;
    using QuizHut.Services.Quiz;
    using QuizHut.Web.Filters;
    using QuizHut.Web.ViewModels.Categories;
    using QuizHut.Web.ViewModels.Quizzes;

    public class CategoriesController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICategoryService service;
        private readonly IQuizService quizService;

        public CategoriesController(
            UserManager<ApplicationUser> userManager,
            ICategoryService service,
            IQuizService quizService)
        {
            this.userManager = userManager;
            this.service = service;
            this.quizService = quizService;
        }

        public async Task<IActionResult> AllCategoriesCreatedByUser()
        {
            var userId = this.userManager.GetUserId(this.User);
            var categories = await this.service.GetAllByCreatorIdAsync<CategoryViewModel>(userId);
            var model = new CategoriesListAllViewModel() { Categories = categories };
            return this.View(model);
        }

        [HttpPost]
        public IActionResult AllCategoriesCreatedByUser(CategoriesListAllViewModel model)
        {
            return this.View(model);
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> Create(CreateCategoryInputViewModel model)
        {
            var userId = this.userManager.GetUserId(this.User);
            var categoryId = await this.service.CreateAsync(model.Name, userId);

            return this.RedirectToAction("AssignQuizzesToCategory", new { id = categoryId });
        }

        public async Task<IActionResult> AssignQuizzesToCategory(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var quizzes = await this.quizService.GetAllAsync<QuizAssignViewModel>();
            quizzes = await this.service.FilterQuizzesThatAreNotAssignedToThisCategory(id, quizzes);
            var model = await this.service.CreateCategoryModelAsync(id, userId, quizzes);

            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AssignQuizzesToCategory(CategoryWithQuizzesViewModel model)
        {
            var quizzesIds = model.Quizzes.Where(x => x.IsAssigned).Select(x => x.Id).ToList();
            await this.service.AssignQuizzesToCategoryAsync(model.Id, quizzesIds);
            return this.RedirectToAction("CategoryDetails", new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> CategoryDetails(string id)
        {
            var model = await this.service.GetCategoryModelAsync(id);
            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            await this.service.DeleteAsync(id);
            return this.RedirectToAction("AllCategoriesCreatedByUser");
        }


        public IActionResult EditName(string id, string name)
        {
            var model = new EditCategoryNameInputViewModel() { Id = id, Name = name };

            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> EditName(EditCategoryNameInputViewModel model)
        {
            await this.service.UpdateNameAsync(model.Id, model.Name);

            return this.RedirectToAction("CategoryDetails", new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuizFromCategory(string categoryId, string quizId)
        {
            await this.service.DeleteQuizFromCategoryAsync(categoryId, quizId);
            return this.RedirectToAction("CategoryDetails", new { id = categoryId });
        }
    }
}