namespace QuizHut.Services.StudentsGroups
{
    using System.Threading.Tasks;

    public interface IStudentsGroupsService
    {
        Task CreateAsync(string groupId, string studentId);

        Task DeleteAsync(string groupId, string studentId);

        Task<string[]> GetAllStudentsIdsByGroupIdAsync(string groupId);
    }
}
