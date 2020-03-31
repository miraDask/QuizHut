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
        private readonly string firstCategoryId;

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
            this.firstCategoryId = this.CreateFirstCategory();
        }

        [Fact]
        public async Task CreateCategoryAsyncShouldCreateNewCategoryInDb()
        {
            var creatorId = Guid.NewGuid().ToString();

            var newCategoryId = await this.service.CreateCategoryAsync(name: "Second category", creatorId);
            var categoriesCount = this.dbContext.Categories.ToArray().Count();
            Assert.Equal(2, categoriesCount);
            Assert.NotNull(await this.dbContext.Categories.FirstOrDefaultAsync(x => x.Id == newCategoryId));
        }

        //[Fact]
        //public async Task GetAllByCreatorIdAsyncShouldReturnCorrectModelCollection()
        //{
        //    var creatorId = Guid.NewGuid().ToString();
        //    var firstCategory = new Category()
        //    {
        //        Name = "Category 1",
        //        CreatorId = creatorId,
        //    };

        //    var secondCategory = new Category()
        //    {
        //        Name = "Category 2",
        //        CreatorId = creatorId,
        //    };

        //    await this.dbContext.Categories.AddAsync(firstCategory);
        //    await this.dbContext.Categories.AddAsync(secondCategory);
        //    await this.dbContext.SaveChangesAsync();

        //    var firstModel = new CategoryViewModel()
        //    {
        //        Name = firstCategory.Name,
        //        Id = firstCategory.Id,
        //        QuizzesCount = firstCategory.Quizzes.Count().ToString(),
        //        CreatedOn = firstCategory.CreatedOn.ToString("dd/MM/yyyy"),
        //    };

        //    var secondModel = new CategoryViewModel()
        //    {
        //        Name = secondCategory.Name,
        //        Id = secondCategory.Id,
        //        QuizzesCount = secondCategory.Quizzes.Count().ToString(),
        //        CreatedOn = secondCategory.CreatedOn.ToString("dd/MM/yyyy"),
        //    };

        //    var resultModelCollection = await this.service.GetAllByCreatorIdAsync<CategoryViewModel>(creatorId);
        //    Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
        //    Assert.Equal(firstModel.Name, resultModelCollection.Last().Name);
        //    Assert.Equal(firstModel.QuizzesCount, resultModelCollection.Last().QuizzesCount);
        //    Assert.Equal(firstModel.CreatedOn, resultModelCollection.Last().CreatedOn);
        //    Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
        //    Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
        //    Assert.Equal(secondModel.QuizzesCount, resultModelCollection.First().QuizzesCount);
        //    Assert.Equal(secondModel.CreatedOn, resultModelCollection.First().CreatedOn);
        //    Assert.Equal(2, resultModelCollection.Count());
        //}

        [Fact]
        public async Task UpdateNameAsyncShouldUpdateCorrectly()
        {
            await this.service.UpdateNameAsync(this.firstCategoryId, "First test category");
            var updatedCategory = await this.GetFirstCategory();

            Assert.Equal("First test category", updatedCategory.Name);
        }

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            await this.service.DeleteAsync(this.firstCategoryId);

            var categoriesCount = this.dbContext.Categories.Where(x => !x.IsDeleted).ToArray().Count();
            var category = await this.dbContext.Categories.FindAsync(this.firstCategoryId);
            Assert.Equal(0, categoriesCount);
            Assert.True(category.IsDeleted);
        }

        [Fact]
        public async Task GetByIdAsyncShouldReturnCorrectModel()
        {
            var category = await this.GetFirstCategory();
            var model = new CategoryWithQuizzesViewModel()
            {
                Id = category.Id,
                Name = category.Name,
                Error = false,
                Quizzes = new List<QuizAssignViewModel>(),
            };

            var resultModel = await this.service.GetByIdAsync<CategoryWithQuizzesViewModel>(this.firstCategoryId);
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

            var quizzesIdList = new List<string>() { firstQuizId, secondQuizId };
            await this.service.AssignQuizzesToCategoryAsync(this.firstCategoryId, quizzesIdList);
            var categoryQuizzesIds = await this.dbContext
                .Categories
                .Where(x => x.Id == this.firstCategoryId)
                .Select(x => x.Quizzes.Select(x => x.Id))
                .FirstOrDefaultAsync();

            Assert.Equal(2, categoryQuizzesIds.Count());
            Assert.Contains(firstQuizId, categoryQuizzesIds);
            Assert.Contains(secondQuizId, categoryQuizzesIds);
        }

        [Fact]
        public async Task DeleteQuizFromCategoryAsyncShouldUnAssignQuizCorrectly()
        {
            var quizId = this.CreateQuiz(name: "quiz");
            await this.AssignQuizToFirstCategoryAsync(quizId);
            await this.service.DeleteQuizFromCategoryAsync(this.firstCategoryId, quizId);
            var categoryQuizzesIds = await this.dbContext
                .Categories
                .Where(x => x.Id == this.firstCategoryId)
                .Select(x => x.Quizzes.Select(x => x.Id))
                .FirstOrDefaultAsync();

            Assert.Empty(categoryQuizzesIds);
            Assert.DoesNotContain(quizId, categoryQuizzesIds);
        }

        private async Task AssignQuizToFirstCategoryAsync(string quizId)
        {
            var category = await this.GetFirstCategory();
            var quiz = await this.dbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);
            category.Quizzes.Add(quiz);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<Category>(category).State = EntityState.Detached;
            this.dbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
        }

        private async Task<Category> GetFirstCategory()
        => await this.dbContext.Categories.FirstOrDefaultAsync(x => x.Id == this.firstCategoryId);

        private string CreateFirstCategory()
        {
            var creatorId = Guid.NewGuid().ToString();
            var category = new Category()
            {
                Name = "First category",
                CreatorId = creatorId,
            };

            this.dbContext.Categories.Add(category);
            this.dbContext.SaveChanges();
            this.dbContext.Entry<Category>(category).State = EntityState.Detached;
            return category.Id;
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
    }
}
