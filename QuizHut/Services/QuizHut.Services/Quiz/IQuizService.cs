using System.Threading.Tasks;

namespace QuizHut.Services.Quiz
{
    public interface IQuizService
    {
        Task AddNewQuizAsync<TViewmodel>(TViewmodel viewmodel);
    }
}
