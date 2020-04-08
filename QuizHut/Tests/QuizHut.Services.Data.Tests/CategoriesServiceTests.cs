namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data;
    using QuizHut.Data.Models;
    using QuizHut.Data.Repositories;
    using QuizHut.Services.Categories;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Categories;
    using QuizHut.Web.ViewModels.Quizzes;
    using Xunit;


    public class CategoriesServiceTests
    {
        private readonly ApplicationDbContext dbContext;
        private readonly EfDeletableEntityRepository<Category> categoriesRepository;
        private readonly EfDeletableEntityRepository<Quiz> quizzesRepository;
        private readonly CategoriesService service;

        public CategoriesServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            this.dbContext = new ApplicationDbContext(options);
            this.categoriesRepository = new EfDeletableEntityRepository<Category>(this.dbContext);
            this.quizzesRepository = new EfDeletableEntityRepository<Quiz>(this.dbContext);
            this.service = new CategoriesService(this.categoriesRepository, this.quizzesRepository);
            AutoMapperConfig.RegisterMappings(typeof(CategoryViewModel).GetTypeInfo().Assembly);
            AutoMapperConfig.RegisterMappings(typeof(CategoryWithQuizzesViewModel).GetTypeInfo().Assembly);
        }

        [Fact]
        public async Task GetAllCategoriesCountShouldReturnCorrectCount()
        {
            var creatorId = Guid.NewGuid().ToString();
            await this.CreateCategoryAsync("first category", creatorId);
            var categoriesCount = this.service.GetAllCategoriesCount(creatorId);
            Assert.Equal(1, categoriesCount);
        }

        [Fact]
        public async Task CreateCategoryAsyncShouldCreateNewCategoryInDb()
        {
            var creatorId = Guid.NewGuid().ToString();
            await this.CreateCategoryAsync(creatorId);

            var newCategoryId = await this.service.CreateCategoryAsync(name: "Second category", creatorId);
            var categoriesCount = this.dbContext.Categories.ToArray().Count();
            var newCategory = await this.dbContext.Categories.FirstOrDefaultAsync(x => x.Id == newCategoryId);

            Assert.Equal(2, categoriesCount);
            Assert.Equal("Second category", newCategory.Name);
            Assert.Equal(creatorId, newCategory.CreatorId);
        }

        [Fact]
        public async Task GetAllPerPageShouldReturnCorrectModelCollection()
        {
            var creatorId = Guid.NewGuid().ToString();
            var firstCategory = await this.CreateCategoryAsync("Category 1", creatorId);
            var secondCategory = await this.CreateCategoryAsync("Category 2", creatorId);

            var firstModel = new CategoryViewModel()
            {
                Name = firstCategory.Name,
                Id = firstCategory.Id,
                QuizzesCount = firstCategory.Quizzes.Count().ToString(),
                CreatedOn = firstCategory.CreatedOn.ToString("dd/MM/yyyy"),
            };

            var secondModel = new CategoryViewModel()
            {
                Name = secondCategory.Name,
                Id = secondCategory.Id,
                QuizzesCount = secondCategory.Quizzes.Count().ToString(),
                CreatedOn = secondCategory.CreatedOn.ToString("dd/MM/yyyy"),
            };

            var resultModelCollection = await this.service.GetAllPerPage<CategoryViewModel>(1, 2, creatorId);
            Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.Last().Name);
            Assert.Equal(firstModel.QuizzesCount, resultModelCollection.Last().QuizzesCount);
            Assert.Equal(firstModel.CreatedOn, resultModelCollection.Last().CreatedOn);
            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.QuizzesCount, resultModelCollection.First().QuizzesCount);
            Assert.Equal(secondModel.CreatedOn, resultModelCollection.First().CreatedOn);
            Assert.Equal(2, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetAllPerPageShouldSkipCorrectly()
        {
            var creatorId = await this.CreateUserAsync();
            var firstCategory = await this.CreateCategoryAsync("Category 1", creatorId);
            await this.CreateCategoryAsync("Category 2", creatorId);

            var firstModel = new CategoryViewModel()
            {
                Name = firstCategory.Name,
                Id = firstCategory.Id,
                QuizzesCount = firstCategory.Quizzes.Count().ToString(),
                CreatedOn = firstCategory.CreatedOn.ToString("dd/MM/yyyy"),
            };

            var resultModelCollection = await this.service.GetAllPerPage<CategoryViewModel>(2, 1, creatorId);

            Assert.Single(resultModelCollection);
            Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.Last().Name);
            Assert.Equal(firstModel.QuizzesCount, resultModelCollection.Last().QuizzesCount);
            Assert.Equal(firstModel.CreatedOn, resultModelCollection.Last().CreatedOn);
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(1, 10000)]
        public async Task GetAllPerPageShouldTakeCorrectCountPerPage(int page, int countPerPage)
        {
            var creatorId = await this.CreateUserAsync();
            for (int i = 0; i < countPerPage * 2; i++)
            {
                await this.CreateCategoryAsync($"Category {i}", creatorId);
            }

            var resultModelCollection = await this.service.GetAllPerPage<CategoryViewModel>(page, countPerPage, creatorId);

            Assert.Equal(countPerPage, resultModelCollection.Count());
        }

        [Fact]
        public async Task UpdateNameAsyncShouldUpdateCorrectly()
        {
            var category = await this.CreateCategoryAsync("Category");

            await this.service.UpdateNameAsync(category.Id, "First test category");
            var updatedCategory = await this.dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);

            Assert.Equal("First test category", updatedCategory.Name);
        }

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            var firstCategory = await this.CreateCategoryAsync("Category 1");
            await this.service.DeleteAsync(firstCategory.Id);

            var categoriesCount = this.dbContext.Categories.Where(x => !x.IsDeleted).ToArray().Count();
            var category = await this.dbContext.Categories.FindAsync(firstCategory.Id);
            Assert.Equal(0, categoriesCount);
            Assert.True(category.IsDeleted);
        }

        [Fact]
        public async Task GetByIdAsyncShouldReturnCorrectModel()
        {
            var creatorId = await this.CreateUserAsync();
            var category = await this.CreateCategoryAsync("Category 1", creatorId);
            var model = new CategoryWithQuizzesViewModel()
            {
                Id = category.Id,
                Name = category.Name,
                Error = false,
                Quizzes = new List<QuizAssignViewModel>(),
            };

            var resultModel = await this.service.GetByIdAsync<CategoryWithQuizzesViewModel>(category.Id);

            Assert.Equal(model.Id, resultModel.Id);
            Assert.Equal(model.Name, resultModel.Name);
            Assert.Equal(model.Error, resultModel.Error);
            Assert.Equal(model.Quizzes, resultModel.Quizzes);
        }

        [Fact]
        public async Task AssignQuizzesToCategoryAsyncShouldAssignQuizzesCorrectly()
        {
            var firstQuizId = this.CreateQuiz(name: "First quiz");
            var secondQuizId = this.CreateQuiz(name: "Second quiz");
            var firstCategory = await this.CreateCategoryAsync("Category 1");

            var quizzesIdList = new List<string>() { firstQuizId, secondQuizId };
            await this.service.AssignQuizzesToCategoryAsync(firstCategory.Id, quizzesIdList);
            var categoryQuizzesIds = await this.dbContext
                .Categories
                .Where(x => x.Id == firstCategory.Id)
                .Select(x => x.Quizzes.Select(x => x.Id))
                .FirstOrDefaultAsync();

            Assert.Equal(2, categoryQuizzesIds.Count());
            Assert.Contains(firstQuizId, categoryQuizzesIds);
            Assert.Contains(secondQuizId, categoryQuizzesIds);
        }

        [Fact]
        public async Task DeleteQuizFromCategoryAsyncShouldUnAssignQuizCorrectly()
        {
            var firstCategory = await this.CreateCategoryAsync("Category 2");

            var quizId = this.CreateQuiz(name: "quiz");
            await this.AssignQuizToFirstCategoryAsync(quizId);
            await this.service.DeleteQuizFromCategoryAsync(firstCategory.Id, quizId);
            var categoryQuizzesIds = await this.dbContext
                .Categories
                .Where(x => x.Id == firstCategory.Id)
                .Select(x => x.Quizzes.Select(x => x.Id))
                .FirstOrDefaultAsync();
            var quiz = await this.dbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);

            Assert.Empty(categoryQuizzesIds);
            Assert.Null(quiz.CategoryId);
        }

        private async Task AssignQuizToFirstCategoryAsync(string quizId)
        {
            var category = await this.CreateCategoryAsync("Category");
            var quiz = await this.dbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);
            category.Quizzes.Add(quiz);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<Category>(category).State = EntityState.Detached;
            this.dbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
        }

        private async Task<Category> CreateCategoryAsync(string name, string creatorId = null)
        {
            if (creatorId == null)
            {
                creatorId = Guid.NewGuid().ToString();
            }

            var category = new Category()
            {
                Name = name,
                CreatorId = creatorId,
            };

            await this.dbContext.Categories.AddAsync(category);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<Category>(category).State = EntityState.Detached;
            return category;
        }

        private string CreateQuiz(string name)
        {
            var creatorId = Guid.NewGuid().ToString();
            var quiz = new Quiz()
            {
                Name = name,
                CreatorId = creatorId,
            };

            this.dbContext.Quizzes.Add(quiz);
            this.dbContext.SaveChanges();
            this.dbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
            return quiz.Id;
        }

        private async Task<string> CreateUserAsync()
        {
            var user = new ApplicationUser()
            {
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "email@email.com",
                UserName = "email@email.com",
            };

            await this.dbContext.Users.AddAsync(user);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<ApplicationUser>(user).State = EntityState.Detached;
            return user.Id;
        }
    }
}
