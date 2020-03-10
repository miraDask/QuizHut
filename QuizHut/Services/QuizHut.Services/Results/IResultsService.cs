namespace QuizHut.Services.Results
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IResultsService
    {
        Task<string> CreateResultAsync(string studentId, int points, int maxPoints);
    }
}
