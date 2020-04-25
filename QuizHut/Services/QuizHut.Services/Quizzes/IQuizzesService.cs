namespace QuizHut.Services.Quizzes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQuizzesService
    {
        Task<string> CreateQuizAsync(string name, string description, int? timer, string creatorId, string password);

        Task<IList<T>> GetAllUnAssignedToEventAsync<T>(string creatorId = null);

        Task<IEnumerable<T>> GetAllPerPageAsync<T>(
            int page,
            int countPerPage,
            string creatorId = null,
            string searchCriteria = null,
            string searchText = null,
            string categoryId = null);

        Task<IList<T>> GetAllByCategoryIdAsync<T>(string id);

        Task<T> GetQuizByIdAsync<T>(string id);

        Task<string> GetCreatorIdByQuizIdAsync(string id);

        Task<string> GetQuizIdByPasswordAsync(string password);

        Task<string> GetQuizNameByIdAsync(string id);

        Task DeleteByIdAsync(string id);

        Task DeleteEventFromQuizAsync(string eventId, string quizId);

        Task UpdateAsync(string id, string name, string description, int? timer, string password);

        Task<IList<T>> GetUnAssignedToCategoryByCreatorIdAsync<T>(string categoryId, string creatorId);

        Task<bool[]> HasUserPermition(string userId, string quizId, bool isQuizTaken);

        Task AssignQuizToEventAsync(string eventId, string quizId);

        Task<int> GetAllQuizzesCountAsync(string creatorId = null, string searchCriteria = null, string searchText = null, string categoryId = null);
    }
}
