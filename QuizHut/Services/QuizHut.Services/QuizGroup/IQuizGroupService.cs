namespace QuizHut.Services.QuizGroup
{
    using System.Threading.Tasks;

    public interface IQuizGroupService
    {
        Task CreateAsync(string groupId, string quizId);

        Task DeleteAsync(string groupId, string quizId);
    }
}
