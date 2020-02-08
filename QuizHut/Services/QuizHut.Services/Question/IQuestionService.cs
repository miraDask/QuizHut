namespace QuizHut.Services.Question
{
    using System.Linq;
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Question;
    using QuizHut.Web.ViewModels.Quiz;

    public interface IQuestionService
    {
        Task<string> AddNewQuestionAsync(QuestionViewModel questionModel);

        // Task<string> GetQuizIdByQuestionIdAsync(string id);

        IOrderedQueryable<AttemtedQuizQuestionViewModel> GetAllQuestionsQuizById(string id);
    }
}
