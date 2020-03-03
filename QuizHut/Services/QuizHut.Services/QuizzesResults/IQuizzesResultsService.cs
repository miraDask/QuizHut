namespace QuizHut.Services.QuizzesResults
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Questions;
    using QuizHut.Web.ViewModels.Quizzes;

    public interface IQuizzesResultsService
    {
        Task CreateQuizResultAsync(string studentId, int points, int maxPoints, string quizId);

        Task<QuizResultViewModel> GetResultModel(
            string quizId,
            string participantId,
            IList<QuestionViewModel> originalQuizQuestions,
            IList<AttemtedQuizQuestionViewModel> attemptedQuizQuestions);
    }
}
