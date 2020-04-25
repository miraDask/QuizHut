namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Data.Models;
    using QuizHut.Services.Categories;
    using QuizHut.Services.Quizzes;
    using QuizHut.Web.Infrastructure.Filters;
    using QuizHut.Web.Infrastructure.Helpers;
    using QuizHut.Web.ViewModels.Categories;
    using QuizHut.Web.ViewModels.Quizzes;

    public class CategoriesController : AdministrationController
    {
        private const int PerPageDefaultValue = 5;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICategoriesService service;
        private readonly IQuizzesService quizService;
        private readonly IDateTimeConverter dateTimeConverter;

        public CategoriesController(
            UserManager<ApplicationUser> userManager,
            ICategoriesService service,
            IQuizzesService quizService,
            IDateTimeConverter dateTimeConverter)
        {
            this.userManager = userManager;
            this.service = service;
            this.quizService = quizService;
            this.dateTimeConverter = dateTimeConverter;
        }

        [ClearDashboardRequestInSessionActionFilterAttribute]
        public async Task<IActionResult> AllCategoriesCreatedByTeacher(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var userId = this.userManager.GetUserId(this.User);

            var model = new CategoriesListAllViewModel()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allCategoriesCreatedByTeacherCount = await this.service.GetAllCategoriesCountAsync(userId, searchCriteria, searchText);
            if (allCategoriesCreatedByTeacherCount > 0)
            {
                var categories = await this.service.GetAllPerPage<CategoryViewModel>(page, countPerPage, userId, searchCriteria, searchText);
                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
                foreach (var category in categories)
                {
                    category.CreatedOnDate = this.dateTimeConverter.GetDate(category.CreatedOn, timeZoneIana);
                }

                model.Categories = categories;
                model.PagesCount = (int)Math.Ceiling(allCategoriesCreatedByTeacherCount / (decimal)countPerPage);
            }

            return this.View(model);
        }

        [HttpPost]
        public IActionResult AllCategoriesCreatedByTeacher(CategoriesListAllViewModel model)
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
            var categoryId = await this.service.CreateCategoryAsync(model.Name, userId);

            return this.RedirectToAction("AssignQuizzesToCategory", new { id = categoryId });
        }

        public async Task<IActionResult> AssignQuizzesToCategory(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var quizzes = await this.quizService.GetUnAssignedToCategoryByCreatorIdAsync<QuizAssignViewModel>(id, userId);
            var model = await this.service.GetByIdAsync<CategoryWithQuizzesViewModel>(id);
            model.Quizzes = quizzes;

            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AssignQuizzesToCategory(CategoryWithQuizzesViewModel model)
        {
            var quizzesIds = model.Quizzes.Where(x => x.IsAssigned).Select(x => x.Id).ToList();

            if (quizzesIds.Count() == 0)
            {
                model.Error = true;
                return this.View(model);
            }

            await this.service.AssignQuizzesToCategoryAsync(model.Id, quizzesIds);
            return this.RedirectToAction("CategoryDetails", new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> CategoryDetails(string id)
        {
            var quizzes = await this.quizService.GetAllByCategoryIdAsync<QuizAssignViewModel>(id);
            var model = await this.service.GetByIdAsync<CategoryWithQuizzesViewModel>(id);
            model.Quizzes = quizzes;

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            await this.service.DeleteAsync(id);
            return this.RedirectToAction("AllCategoriesCreatedByTeacher");
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
