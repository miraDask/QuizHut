namespace QuizHut.Services.Quiz
{
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Quiz;

    public interface IQuizService
    {
        Task<string> AddNewQuizAsync(InputQuizViewModel quizModel);

        bool GetDublicatedQuizPassword(string quizPassword);
    }
}
