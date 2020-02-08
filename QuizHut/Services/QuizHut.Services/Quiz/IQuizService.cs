namespace QuizHut.Services.Quiz
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Quiz;

    public interface IQuizService
    {
        Task<string> AddNewQuizAsync(InputQuizViewModel quizModel);

        Task<IEnumerable<T>> GetAllAsync<T>();

        Task<T> GetQuizByIdAsync<T>(string id);
    }
}
