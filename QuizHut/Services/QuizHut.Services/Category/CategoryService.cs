namespace QuizHut.Services.Category
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Categories;
    using QuizHut.Web.ViewModels.Quizzes;

    public class CategoryService : ICategoryService
    {
        private readonly IDeletableEntityRepository<Category> repository;
        private readonly IDeletableEntityRepository<Quiz> quizRepository;

        public CategoryService(
            IDeletableEntityRepository<Category> repository,
            IDeletableEntityRepository<Quiz> quizRepository)
        {
            this.repository = repository;
            this.quizRepository = quizRepository;
        }

        public async Task<string> CreateAsync(string name, string creatorId)
        {
            var category = new Category() { Name = name, CreatorId = creatorId };
            await this.repository.AddAsync(category);
            await this.repository.SaveChangesAsync();
            return category.Id;
        }

        public async Task<IList<T>> GetAllByCreatorIdAsync<T>(string id)
         => await this.repository
                .AllAsNoTracking()
                .Where(x => x.CreatorId == id)
                .OrderByDescending(x => x.CreatedOn)
                .To<T>()
                .ToListAsync();

        public async Task<CategoryWithQuizzesViewModel> CreateCategoryModelAsync(string id, string creatorId, IList<QuizAssignViewModel> quizzes)
        {
            var category = await this.repository.AllAsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            var model = new CategoryWithQuizzesViewModel()
            {
                Id = id,
                Name = category.Name,
                Quizzes = quizzes.Where(x => x.CreatorId == creatorId).ToList(),
            };

            return model;
        }

        public async Task AssignQuizzesToCategoryAsync(string id, List<string> quizzesIds)
        {
            var category = await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            foreach (var quizId in quizzesIds)
            {
                var quiz = await this.quizRepository
                    .AllAsNoTracking()
                    .Where(x => x.Id == quizId)
                    .FirstOrDefaultAsync();
                category.Quizzes.Add(quiz);
                quiz.CategoryId = id;
                this.repository.Update(category);
                this.quizRepository.Update(quiz);
            }

            await this.repository.SaveChangesAsync();
            await this.quizRepository.SaveChangesAsync();
        }

        public async Task<CategoryWithQuizzesViewModel> GetCategoryModelAsync(string id)
        {
            var category = await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CategoryWithQuizzesViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Quizzes = x.Quizzes.Select(x => new QuizAssignViewModel()
                    {
                        Name = x.Name,
                        Id = x.Id,
                    }).ToList(),
                })
                .FirstOrDefaultAsync();

            return category;
        }

        public async Task DeleteAsync(string id)
        {
            var category = await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            this.repository.Delete(category);
            await this.repository.SaveChangesAsync();
        }

        public async Task<IList<QuizAssignViewModel>> FilterQuizzesThatAreNotAssignedToThisCategory(string id, IList<QuizAssignViewModel> quizzes)
        {
            var assignedQuizzesIds = await this.quizRepository.AllAsNoTracking().Where(x => x.CategoryId == id).Select(x => x.Id).ToListAsync();
            return quizzes.Where(x => !assignedQuizzesIds.Contains(x.Id)).ToList();
        }

        public async Task UpdateNameAsync(string id, string newName)
        {
            var category = await this.repository.AllAsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            category.Name = newName;
            this.repository.Update(category);
            await this.repository.SaveChangesAsync();
        }

        public async Task DeleteQuizFromCategoryAsync(string categoryId, string quizId)
        {
            var category = await this.repository
              .AllAsNoTracking()
              .Where(x => x.Id == categoryId)
              .FirstOrDefaultAsync();

            var quiz = await this.quizRepository
                    .AllAsNoTracking()
                    .Where(x => x.Id == quizId)
                    .FirstOrDefaultAsync();

            category.Quizzes.Remove(quiz);
            quiz.CategoryId = null;
            this.repository.Update(category);
            this.quizRepository.Update(quiz);
            await this.repository.SaveChangesAsync();
            await this.quizRepository.SaveChangesAsync();
        }
    }
}
