namespace QuizHut.Services.EventsResults
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Questions;
    using QuizHut.Web.ViewModels.Quizzes;

    public interface IEventsResultsService
    {
        Task CreateEventResultAsync(string studentId, int points, int maxPoints, string quizId);

        Task<QuizResultViewModel> GetResultModel(
            string quizId,
            string studentId,
            IList<QuestionViewModel> originalQuizQuestions,
            IList<AttemtedQuizQuestionViewModel> attemptedQuizQuestions);
    }
}
