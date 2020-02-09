namespace QuizHut.Services.QuizResult
{
    using System.Threading.Tasks;

    public interface IQuizResultService
    {
        Task SaveQuizResult(string participantId, string quizId, int points, int maxPoints);
    }
}
