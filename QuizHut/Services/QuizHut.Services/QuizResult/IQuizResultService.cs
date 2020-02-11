namespace QuizHut.Services.QuizResult
{
    using System.Threading.Tasks;

    public interface IQuizResultService
    {
        Task CreateQuizResultAsync(string participantId, int points, int maxPoints, string quizId);
    }
}
