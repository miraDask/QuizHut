namespace QuizHut.Services.Categories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICategoriesService
    {
        Task<IEnumerable<T>> GetAllPerPage<T>(int page, int countPerPage, string creatorId);

        Task<T> GetByIdAsync<T>(string id);

        Task<string> CreateCategoryAsync(string name, string creatorId);

        Task AssignQuizzesToCategoryAsync(string id, IEnumerable<string> quizzesIds);

        Task DeleteAsync(string id);

        Task UpdateNameAsync(string id, string newName);

        Task DeleteQuizFromCategoryAsync(string categoryId, string quizId);

        int GetAllCategoriesCount(string creatorId);
    }
}
