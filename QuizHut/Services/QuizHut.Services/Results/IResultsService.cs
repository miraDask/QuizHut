namespace QuizHut.Services.Results
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Questions;
    using QuizHut.Web.ViewModels.Quizzes;

    public interface IResultsService
    {
        Task CreateResultAsync(string studentId, int points, int maxPoints, string quizId);

        Task<IEnumerable<T>> GetAllByStudentIdAsync<T>(string id);

        Task<IEnumerable<T>> GetAllResultsByEventIdAsync<T>(string eventId, string groupName);

        Task<QuizResultViewModel> GetResultModel(
           string quizId,
           string studentId,
           IList<QuestionViewModel> originalQuizQuestions,
           IList<AttemtedQuizQuestionViewModel> attemptedQuizQuestions);
    }
}
