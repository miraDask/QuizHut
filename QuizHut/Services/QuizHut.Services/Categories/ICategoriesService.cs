namespace QuizHut.Services.Categories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Categories;
    using QuizHut.Web.ViewModels.Quizzes;

    public interface ICategoriesService
    {
        Task<IList<T>> GetAllByCreatorIdAsync<T>(string id);

        Task<T> GetByIdAsync<T>(string id);

        Task<string> CreateCategoryAsync(string name, string creatorId);

        Task AssignQuizzesToCategoryAsync(string id, List<string> quizzesIds);

        Task DeleteAsync(string id);

        Task UpdateNameAsync(string id, string newName);

        Task DeleteQuizFromCategoryAsync(string categoryId, string quizId);
    }
}
