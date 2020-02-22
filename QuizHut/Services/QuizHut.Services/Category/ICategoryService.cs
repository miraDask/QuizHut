namespace QuizHut.Services.Category
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Categories;
    using QuizHut.Web.ViewModels.Quizzes;

    public interface ICategoryService
    {
        Task<IList<T>> GetAllByCreatorIdAsync<T>(string id);

        Task<string> CreateAsync(string name, string creatorId);

        Task<CategoryWithQuizzesViewModel> CreateCategoryModelAsync(string id, string creatorId, IList<QuizAssignViewModel> quizzes);

        Task<CategoryWithQuizzesViewModel> GetCategoryModelAsync(string id);

        Task AssignQuizzesToCategoryAsync(string id, List<string> quizzesIds);

        Task DeleteAsync(string id);

        Task UpdateNameAsync(string id, string newName);

        Task DeleteQuizFromCategoryAsync(string categoryId, string quizId);

        Task<IList<QuizAssignViewModel>> FilterQuizzesThatAreNotAssignedToThisCategory(string id, IList<QuizAssignViewModel> quizzes);
    }
}
