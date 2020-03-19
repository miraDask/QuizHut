namespace QuizHut.Services.Results
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IResultsService
    {
        Task CreateResultAsync(string studentId, int points, int maxPoints, string quizId);

        Task<IEnumerable<T>> GetAllByStudentIdAsync<T>(string id);

        Task<IEnumerable<T>> GetAllResultsByEventIdAsync<T>(string eventId, string groupName);

       // Task<bool> ResultExists(string eventId, string studentId);
    }
}
