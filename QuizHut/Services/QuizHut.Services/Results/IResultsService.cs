namespace QuizHut.Services.Results
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IResultsService
    {
        Task<string> CreateResultAsync(string studentId, int points, int maxPoints, string quizId);

        Task<IEnumerable<T>> GetAllByStudentIdAsync<T>(string id);

        int GetResultsCountByStudentId(string id);

        int GetAllResultsByEventAndGroupCount(string eventId, string groupId);

        Task<string> GetQuizNameByEventIdAndStudentIdAsync(string eventId, string studentId);

        Task<IEnumerable<T>> GetPerPageByStudentIdAsync<T>(string id, int page, int countPerPage);

        Task<IEnumerable<T>> GetAllResultsByEventAndGroupPerPageAsync<T>(string eventId, string groupId, int page, int countPerPage);
    }
}
