namespace QuizHut.Services.Quizzes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQuizzesService
    {
        Task<string> CreateQuizAsync(string name, string description, int? timer, string creatorId, string password);

        Task<IList<T>> GetAllUnAssignedToEventAsync<T>(string creatorId = null);

        Task<IEnumerable<T>> GetAllPerPage<T>(int page, int countPerPage, string creatorId = null);

        Task<IList<T>> GetAllByCategoryIdAsync<T>(string id);

        Task<T> GetQuizByIdAsync<T>(string id);

        Task<string> GetCreatorIdByQuizIdAsync(string id);

        Task<string> GetQuizIdByPasswordAsync(string password);

        Task<string> GetQuizNameByIdAsync(string id);

        Task DeleteByIdAsync(string id);

        Task DeleteEventFromQuiz(string eventId, string quizId);

        Task UpdateAsync(string id, string name, string description, int? timer, string password);

        Task<IList<T>> GetUnAssignedToCategoryByCreatorIdAsync<T>(string categoryId, string creatorId);

        Task<bool> HasUserPermition(string userId, string quizId);

        Task AssignEventToQuiz(string eventId, string quizId);

        int GetAllQuizzesCount(string creatorId = null);
    }
}
