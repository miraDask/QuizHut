namespace QuizHut.Services.Quizzes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQuizzesService
    {
        Task<string> CreateQuizAsync(string name, string description, int? timer, string creatorId, string password);

        Task<IList<T>> GetAllAsync<T>();

        Task<IList<T>> GetAllByCreatorIdAsync<T>(string id, bool isAssignedToEventFilter);

        Task<IList<T>> GetAllByCategoryIdAsync<T>(string id);

        Task<T> GetQuizByIdAsync<T>(string id);

        Task<string> GetQuizIdByPasswordAsync(string password);

        Task DeleteByIdAsync(string id);

        Task DeleteEventFromQuiz(string eventId, string quizId);

        Task UpdateAsync(string id, string name, string description, int? timer, string password);

        Task<IList<T>> GetUnAssignedToCategoryByCreatorIdAsync<T>(string categoryId, string creatorId);

        Task<bool> HasUserPermition(string userId, string quizId);

        Task AssignEventToQuiz(string eventId, string quizId);
    }
}
