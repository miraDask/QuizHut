namespace QuizHut.Services.QuizzesGroups
{
    using System.Threading.Tasks;

    public interface IQuizzesGroupsService
    {
        Task CreateAsync(string groupId, string quizId);

        Task DeleteAsync(string groupId, string quizId);

        Task<string[]> GetAllQizzesIdsByGroupIdAsync(string groupId);
    }
}
