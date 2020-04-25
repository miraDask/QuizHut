namespace QuizHut.Services.Results
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IResultsService
    {
        Task<string> CreateResultAsync(string studentId, int points, int maxPoints, string quizId);

        Task<IEnumerable<T>> GetAllByStudentIdAsync<T>(string id);

        Task<int> GetResultsCountByStudentIdAsync(string id, string searchCriteria = null, string searchText = null);

        Task<int> GetAllResultsByEventAndGroupCountAsync(string eventId, string groupId);

        Task<string> GetQuizNameByEventIdAndStudentIdAsync(string eventId, string studentId);

        Task<IEnumerable<T>> GetPerPageByStudentIdAsync<T>(string id, int page, int countPerPage, string searchCriteria = null, string searchText = null);

        Task<IEnumerable<T>> GetAllResultsByEventAndGroupPerPageAsync<T>(string eventId, string groupId, int page, int countPerPage);
    }
}
