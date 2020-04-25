namespace QuizHut.Services.Categories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICategoriesService
    {
        Task<IEnumerable<T>> GetAllPerPage<T>(int page, int countPerPage, string creatorId, string searchCriteria = null, string searchText = null);

        Task<T> GetByIdAsync<T>(string id);

        Task<string> CreateCategoryAsync(string name, string creatorId);

        Task AssignQuizzesToCategoryAsync(string id, IEnumerable<string> quizzesIds);

        Task<IEnumerable<T>> GetAllByCreatorIdAsync<T>(string creatorId);

        Task DeleteAsync(string id);

        Task UpdateNameAsync(string id, string newName);

        Task DeleteQuizFromCategoryAsync(string categoryId, string quizId);

        Task<int> GetAllCategoriesCountAsync(string creatorId, string searchCriteria = null, string searchText = null);
    }
}
