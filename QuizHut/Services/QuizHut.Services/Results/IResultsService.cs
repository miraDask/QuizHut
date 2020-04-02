namespace QuizHut.Services.Results
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IResultsService
    {
        Task<string> CreateResultAsync(string studentId, int points, int maxPoints, string quizId);

        Task<IEnumerable<T>> GetAllByStudentIdAsync<T>(string id);

        int GetResultsCountByStudentId(string id);

        Task<string> GetQuizNameByEventIdAndStudentIdAsync(string eventId, string studentId);

        Task<IEnumerable<T>> GetByStudentIdAsync<T>(string id, int page, int countPerPage);

        Task<IEnumerable<T>> GetAllResultsByEventIdAsync<T>(string eventId, string groupName);
    }
}
