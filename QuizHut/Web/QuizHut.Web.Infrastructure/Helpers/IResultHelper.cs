namespace QuizHut.Web.Infrastructure.Helpers
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Questions;
    using QuizHut.Web.ViewModels.Quizzes;

    public interface IResultHelper
    {
        int CalculateResult(IList<QuestionViewModel> originalQuizQuestions, IList<AttemtedQuizQuestionViewModel> attemptedQuizQuestions);
    }
}
