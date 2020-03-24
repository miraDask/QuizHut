namespace QuizHut.Services.Data.Tests
{
    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data;
    using QuizHut.Data.Models;
    using QuizHut.Data.Repositories;
    using QuizHut.Services.Categories;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Categories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
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

        [Fact]
        public async Task GetAllByCreatorIdAsyncShouldReturnCorrectModelCollection()
        {
            var creatorId = Guid.NewGuid().ToString();
            var firstCategory = new Category()
            {
                Name = "Category 1",
                CreatorId = creatorId,
            };

            var secondCategory = new Category()
            {
                Name = "Category 2",
                CreatorId = creatorId,
            };

            await this.dbContext.Categories.AddAsync(firstCategory);
            await this.dbContext.Categories.AddAsync(secondCategory);
            await this.dbContext.SaveChangesAsync();

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

            var resultModelCollection = await this.service.GetAllByCreatorIdAsync<CategoryViewModel>(creatorId);
            Assert.Equal(firstModel.Id, resultModelCollection[1].Id);
            Assert.Equal(firstModel.Name, resultModelCollection[1].Name);
            Assert.Equal(firstModel.QuizzesCount, resultModelCollection[1].QuizzesCount);
            Assert.Equal(firstModel.CreatedOn, resultModelCollection[1].CreatedOn);
            Assert.Equal(secondModel.Id, resultModelCollection[0].Id);
            Assert.Equal(secondModel.Name, resultModelCollection[0].Name);
            Assert.Equal(secondModel.QuizzesCount, resultModelCollection[0].QuizzesCount);
            Assert.Equal(secondModel.CreatedOn, resultModelCollection[0].CreatedOn);
            Assert.Equal(2, resultModelCollection.Count());
        }

        [Fact]
        public async Task UpdateNameAsyncShouldUpdateCorrectly()
        {
            await this.service.UpdateNameAsync(this.firstCategoryId, "First test category");
            var updatedCategory = await this.dbContext.Categories.FirstOrDefaultAsync(x => x.Id == this.firstCategoryId);

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
    }
}
