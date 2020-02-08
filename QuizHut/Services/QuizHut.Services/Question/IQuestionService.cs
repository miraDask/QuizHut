namespace QuizHut.Services.Question
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using QuizHut.Data.Models;
    using QuizHut.Web.ViewModels.Question;

    public interface IQuestionService
    {
        Task<string> AddNewQuestionAsync(QuestionViewModel questionModel);

        // Task<string> GetQuizIdByQuestionIdAsync(string id);

        IOrderedQueryable<QuestionViewModel> GetAllQuestionsQuizById(string id);
    }
}
