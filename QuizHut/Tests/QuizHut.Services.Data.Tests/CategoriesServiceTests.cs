namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using QuizHut.Data.Models;
    using QuizHut.Services.Categories;
    using QuizHut.Web.ViewModels.Categories;
    using QuizHut.Web.ViewModels.Quizzes;
    using Xunit;

    public class CategoriesServiceTests : BaseServiceTests
    {
        private ICategoriesService Service => this.ServiceProvider.GetRequiredService<ICategoriesService>();

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

            var resultModel = await this.Service.GetByIdAsync<CategoryWithQuizzesViewModel>(category.Id);

            Assert.Equal(model.Id, resultModel.Id);
            Assert.Equal(model.Name, resultModel.Name);
            Assert.Equal(model.Error, resultModel.Error);
            Assert.Equal(model.Quizzes.Count, resultModel.Quizzes.Count);
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
                CreatedOn = firstCategory.CreatedOn,
            };

            var secondModel = new CategoryViewModel()
            {
                Name = secondCategory.Name,
                Id = secondCategory.Id,
                QuizzesCount = secondCategory.Quizzes.Count().ToString(),
                CreatedOn = secondCategory.CreatedOn,
            };

            var resultModelCollection = await this.Service.GetAllPerPage<CategoryViewModel>(1, 2, creatorId);
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
        public async Task GetAllByCreatorIdAsyncShouldReturnCorrectModelCollection()
        {
            var creatorId = Guid.NewGuid().ToString();
            var firstCategory = await this.CreateCategoryAsync("Category 1", creatorId);
            var secondCategory = await this.CreateCategoryAsync("Category 2", creatorId);

            var firstModel = new CategoryViewModel()
            {
                Name = firstCategory.Name,
                Id = firstCategory.Id,
                QuizzesCount = firstCategory.Quizzes.Count().ToString(),
                CreatedOn = firstCategory.CreatedOn,
            };

            var secondModel = new CategoryViewModel()
            {
                Name = secondCategory.Name,
                Id = secondCategory.Id,
                QuizzesCount = secondCategory.Quizzes.Count().ToString(),
                CreatedOn = secondCategory.CreatedOn,
            };

            var resultModelCollection = await this.Service.GetAllPerPage<CategoryViewModel>(1, 2, creatorId);
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
        public async Task GetAllCategoriesCountShouldReturnCorrectCount()
        {
            var creatorId = Guid.NewGuid().ToString();
            await this.CreateCategoryAsync("first category", creatorId);
            var categoriesCount = await this.Service.GetAllCategoriesCountAsync(creatorId);
            Assert.Equal(1, categoriesCount);
        }

        [Fact]
        public async Task CreateCategoryAsyncShouldCreateNewCategoryInDb()
        {
            var creatorId = Guid.NewGuid().ToString();
            await this.CreateCategoryAsync(creatorId);

            var newCategoryId = await this.Service.CreateCategoryAsync(name: "Second category", creatorId);
            var categoriesCount = this.DbContext.Categories.ToArray().Count();
            var newCategory = await this.DbContext.Categories.FirstOrDefaultAsync(x => x.Id == newCategoryId);

            Assert.Equal(2, categoriesCount);
            Assert.Equal("Second category", newCategory.Name);
            Assert.Equal(creatorId, newCategory.CreatorId);
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
                CreatedOn = firstCategory.CreatedOn,
            };

            var resultModelCollection = await this.Service.GetAllPerPage<CategoryViewModel>(2, 1, creatorId);

            Assert.Single(resultModelCollection);
            Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.Last().Name);
            Assert.Equal(firstModel.QuizzesCount, resultModelCollection.Last().QuizzesCount);
            Assert.Equal(firstModel.CreatedOn, resultModelCollection.Last().CreatedOn);
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(1, 1000)]
        public async Task GetAllPerPageShouldTakeCorrectCountPerPage(int page, int countPerPage)
        {
            var creatorId = await this.CreateUserAsync();
            for (int i = 0; i < countPerPage * 2; i++)
            {
                await this.CreateCategoryAsync($"Category {i}", creatorId);
            }

            var resultModelCollection = await this.Service.GetAllPerPage<CategoryViewModel>(page, countPerPage, creatorId);

            Assert.Equal(countPerPage, resultModelCollection.Count());
        }

        [Fact]
        public async Task UpdateNameAsyncShouldUpdateCorrectly()
        {
            var category = await this.CreateCategoryAsync("Category");

            await this.Service.UpdateNameAsync(category.Id, "First test category");
            var updatedCategory = await this.DbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);

            Assert.Equal("First test category", updatedCategory.Name);
        }

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            var firstCategory = await this.CreateCategoryAsync("Category 1");
            await this.Service.DeleteAsync(firstCategory.Id);

            var categoriesCount = this.DbContext.Categories.Where(x => !x.IsDeleted).ToArray().Count();
            var category = await this.DbContext.Categories.FindAsync(firstCategory.Id);
            Assert.Equal(0, categoriesCount);
            Assert.True(category.IsDeleted);
        }

        [Fact]
        public async Task AssignQuizzesToCategoryAsyncShouldAssignQuizzesCorrectly()
        {
            var firstQuizId = this.CreateQuiz(name: "First quiz");
            var secondQuizId = this.CreateQuiz(name: "Second quiz");
            var firstCategory = await this.CreateCategoryAsync("Category 1");

            var quizzesIdList = new List<string>() { firstQuizId, secondQuizId };
            await this.Service.AssignQuizzesToCategoryAsync(firstCategory.Id, quizzesIdList);
            var categoryQuizzesIds = await this.DbContext
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
            await this.Service.DeleteQuizFromCategoryAsync(firstCategory.Id, quizId);
            var categoryQuizzesIds = await this.DbContext
                .Categories
                .Where(x => x.Id == firstCategory.Id)
                .Select(x => x.Quizzes.Select(x => x.Id))
                .FirstOrDefaultAsync();
            var quiz = await this.DbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);

            Assert.Empty(categoryQuizzesIds);
            Assert.Null(quiz.CategoryId);
        }

        private async Task AssignQuizToFirstCategoryAsync(string quizId)
        {
            var category = await this.CreateCategoryAsync("Category");
            var quiz = await this.DbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);
            category.Quizzes.Add(quiz);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Category>(category).State = EntityState.Detached;
            this.DbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
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

            await this.DbContext.Categories.AddAsync(category);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Category>(category).State = EntityState.Detached;
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

            this.DbContext.Quizzes.Add(quiz);
            this.DbContext.SaveChanges();
            this.DbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
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

            await this.DbContext.Users.AddAsync(user);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<ApplicationUser>(user).State = EntityState.Detached;
            return user.Id;
        }
    }
}
